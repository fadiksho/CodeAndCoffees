import { Workbox } from "workbox-window";
import Toast, { ToastType } from "../../components/toast";
import {
  CheckIfSubscriberResponse,
  BeforeInstallPromptEvent
} from "../../types/types";

let swRegisteration: ServiceWorkerRegistration;
let pushNotificationSubscription: PushSubscription;
let deferredPrompt: BeforeInstallPromptEvent;

let pushNotificationButton: HTMLButtonElement;
let pwaInstallButton: HTMLButtonElement;
const vapidPublicKey =
  "BDW5yAPVXetyXAN9C1F31REgmWRM9WlQUUX2r0ZgAPgqN_RlTKwN7eWJDHT0SOVpljZ8db3AyM4O91m4MMOuOTw";



window.addEventListener("load", () => {
  pushNotificationButton = document.getElementById(
    "pushNotificationButton"
  ) as HTMLButtonElement;
  pwaInstallButton = document.getElementById(
    "pwaInstallButton"
  ) as HTMLButtonElement;
  if (window.matchMedia("(display-mode: standalone)").matches) {
    pwaInstallButton.style.display = "none";
  }
});

if ("serviceWorker" in navigator) {
  const wb = new Workbox("/sw.js");
  wb.addEventListener("waiting", event => {
    if (localStorage.getItem("isPromptedToUpdate") === "true") return;
    else localStorage.setItem("isPromptedToUpdate", "true");
    Toast.toast("New Version Is here Update now", {
      onAccept: () => {
        // Assuming the user accepted the update, set up a listener
        // that will reload the page as soon as the previously waiting
        // service worker has taken control.
        wb.addEventListener("controlling", event => {
          window.location.reload();
        });

        // Send a message telling the service worker to skip waiting.
        // This will trigger the `controlling` event handler above.
        // Note: for this to work, you have to add a message
        // listener in your service worker. See below.
        wb.messageSW({ type: "SKIP_WAITING" });
        // localStorage.setItem("isPromptedToUpdate", true);
      },
      sticky: true
    });
  });
  wb.addEventListener("installed", event => {
    if (event.isUpdate) {
      localStorage.setItem("isPromptedToUpdate", "false");
    }
  });
  wb.register().then(async swReg => {
    swRegisteration = swReg;
    if ("PushManager" in window) {
      const sub = await swRegisteration.pushManager.getSubscription();
      if (sub) {
        pushNotificationSubscription = sub;
        if (!isPushNotificationEnabled()) {
          try {
            const isSubscriber = await checkIfSubscriber();
            if (isSubscriber) savePushNotificationResult();
          } catch {
            clearPushNotificationResult();
          } finally {
            updateNotificationButton();
          }
        }
      }
      initializePushNotification();
    }
    updateNotificationButton();
  });
  window.addEventListener(
    "beforeinstallprompt",
    (e: BeforeInstallPromptEvent) => {
      e.preventDefault();
      // Stash the event so it can be triggered later.
      deferredPrompt = e;
      pwaInstallButton.style.display = "inline-block";
      initializeAddToHomeScreen();
    }
  );
}

function updateNotificationButton(): void {
  pushNotificationButton.disabled = true;
  const pushNotificationIcon = pushNotificationButton.firstElementChild.firstElementChild;
  if (isPushNotificationEnabled()) {
    if (pushNotificationIcon.getAttributeNS('http://www.w3.org/1999/xlink', 'href') !== "#icon-bell-silent")
      pushNotificationIcon.setAttribute(
        "xlink:href",
        "#icon-bell-silent"
      );
  } else {
    if (pushNotificationIcon.getAttributeNS('http://www.w3.org/1999/xlink', 'href') !== "#icon-bell")
      pushNotificationIcon.setAttribute(
        "xlink:href",
        "#icon-bell"
      );
  }
  pushNotificationButton.disabled = false;
}

async function checkIfSubscriber() {
  const response = await fetch(
    "/api/subscribers/CheckIfPushNotificationSubscriber",
    {
      method: "POST",
      body: JSON.stringify({
        EndPoint: pushNotificationSubscription.endpoint
      }),
      headers: {
        "Content-Type": "application/json"
      }
    }
  );
  if (response.ok) {
    const checkIfSubscriberResponse = (await response.json()) as CheckIfSubscriberResponse;
    return Promise.resolve(checkIfSubscriberResponse.isSubscribed);
  }
  return Promise.reject(response.status);
}

function initializePushNotification() {
  let isEnabled = true;
  pushNotificationButton.addEventListener("click", async e => {
    if (!isEnabled) return;
    else isEnabled = false;
    if (!pushNotificationSubscription)
      pushNotificationSubscription = await swRegisteration.pushManager.getSubscription();

    if (isPushNotificationEnabled()) {
      Toast.toast("Are you Sure Want to UnSubscribe From Notification?", {
        onAccept: async () => {
          try {
            const response = await fetch(
              "/api/subscribers/PushNotificationCancelSubscription",
              {
                method: "POST",
                body: JSON.stringify({
                  EndPoint: pushNotificationSubscription.endpoint
                }),
                headers: {
                  "Content-Type": "application/json"
                }
              }
            );
            if (response.ok) {
              clearPushNotificationResult();
              Toast.toast("Unsubscribe Succeded  :)", {
                type: ToastType.success
              });
            }
          } catch {
            Toast.toast("Ooops something went wrong! Try again later.", {
              type: ToastType.error
            });
          } finally {
            updateNotificationButton();
            isEnabled = true;
          }
        },
        onReject: () => {
          updateNotificationButton();
          isEnabled = true;
        },
        sticky: true
      });
    } else {
      try {
        await askPermission();
        try {
          const response = await subscribeUser();
          if (response.ok) {
            savePushNotificationResult();
          } else {
            throw new Error();
          }
        } catch {
          Toast.toast("Ooops something went wrong :(", {
            type: ToastType.error
          });
        } finally {
          updateNotificationButton();
        }
      } catch {
        Toast.toast("Please Allow Notification from your browser settings.", {
          type: ToastType.warning,
          sticky: true
        });
      } finally {
        isEnabled = true;
      }
    }
  });
}

function urlBase64ToUint8Array(base64String: string): Uint8Array {
  const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding).replace(/\-/g, "+").replace(/_/g, "/");
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}

function isPushNotificationEnabled(): boolean {
  const isSubscribed =
    localStorage.getItem("PNSubscribedDate") &&
    new Date(localStorage.getItem("PNSubscribedDate")) > new Date();

  return isSubscribed;
}

function savePushNotificationResult(): void {
  const PNSubscribedDate = new Date();
  PNSubscribedDate.setDate(PNSubscribedDate.getDate() + 7);
  localStorage.setItem("PNSubscribedDate", PNSubscribedDate.toString());
}

function clearPushNotificationResult(): void {
  localStorage.removeItem("PNSubscribedDate");
}

async function askPermission(): Promise<void> {
  const permissionResult1 = await new Promise(function (resolve, reject) {
    const permissionResult = Notification.requestPermission(function (result) {
      resolve(result);
    });
    if (permissionResult) {
      permissionResult.then(resolve, reject);
    }
  });
  if (permissionResult1 !== "granted") {
    throw new Error("We weren't granted permission.");
  }
}

function initializeAddToHomeScreen(): void {
  pwaInstallButton.addEventListener("click", () => {
    // Show the prompt
    deferredPrompt.prompt();
    // Wait for the user to respond to the prompt
    deferredPrompt.userChoice.then(choiceResult => {
      if (choiceResult.outcome === "accepted") {
        pwaInstallButton.style.display = "none";
      }
    });
  });
}

async function subscribeUser(): Promise<any> {
  const sub = await swRegisteration.pushManager.subscribe({
    userVisibleOnly: true,
    applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
  });
  const rawKey = sub.getKey ? sub.getKey("p256dh") : null;
  const key = rawKey
    ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawKey)))
    : "";
  const rawAuthSecret = sub.getKey ? sub.getKey("auth") : null;
  const authSecret = rawAuthSecret
    ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawAuthSecret)))
    : "";

  const pushNotificationSubscription = {
    Key: key,
    EndPoint: sub.endpoint,
    AuthSecret: authSecret
  };
  return await fetch("/api/subscribers/PushNotificationSubscription", {
    method: "POST",
    body: JSON.stringify(pushNotificationSubscription),
    headers: {
      "Content-Type": "application/json"
    }
  });
}