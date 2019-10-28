import { Workbox } from "workbox-window";
import Toast from "../js/toast";

let swRegisteration,
  pushNotificationSubscription,
  pushNotificationButton,
  pwaInstallButton,
  deferredPrompt;
const vapidPublicKey =
  "BDW5yAPVXetyXAN9C1F31REgmWRM9WlQUUX2r0ZgAPgqN_RlTKwN7eWJDHT0SOVpljZ8db3AyM4O91m4MMOuOTw";

window.addEventListener("load", function() {
  pushNotificationButton = document.getElementById("pushNotificationButton");
  pwaInstallButton = document.getElementById("pwaInstallButton");
  console.log("Check if is app or browser!");
  if (
    window.matchMedia("(display-mode: standalone)").matches ||
    window.navigator.standalone === true
  ) {
    console.log("From PWA!");
    pwaInstallButton.style.display = "none";
  } else {
    console.log("From Browser!");
    pwaInstallButton.style.display = "inline-block";
  }
});

if ("serviceWorker" in navigator) {
  const wb = new Workbox("/sw.js");
  wb.addEventListener("activated", event => {
    // `event.isUpdate` will be true if another version of the service
    // worker was controlling the page when this version was registered.
  });

  wb.addEventListener("waiting", event => {
    Toast.toast("New Version Is here Update now", {
      sticky: true,
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
      }
    });
  });

  wb.register().then(swReg => {
    swRegisteration = swReg;
    if ("PushManager" in window) {
      swRegisteration.pushManager.getSubscription().then(sub => {
        if (sub) {
          pushNotificationSubscription = sub;
          if (!isPushNotificationEnabled()) {
            checkIfSubscriber()
              .then(isSubscribed => {
                if (isSubscribed) savePushNotificationResult();
                else clearPushNotificationResult();
              })
              .catch(() => {
                clearPushNotificationResult();
              })
              .finally(() => {
                console.log("finally");
                updateNotificationButton();
              });
          }
        }
        initializePushNotification();
      });
    }
    updateNotificationButton();
  });

  window.addEventListener("beforeinstallprompt", function(e) {
    console.log("BeforeInstallPromt");
    e.preventDefault();
    // Stash the event so it can be triggered later.
    deferredPrompt = e;
    pwaInstallButton.style.display = "inline-block";
    initializeAddToHomeScreen();
  });
}

function updateNotificationButton() {
  pushNotificationButton.disabled = true;
  if (isPushNotificationEnabled()) {
    pushNotificationButton.firstElementChild.classList.remove("icon-bell");
    pushNotificationButton.firstElementChild.classList.add("icon-bell-silent");
  } else {
    pushNotificationButton.firstElementChild.classList.add("icon-bell");
    pushNotificationButton.firstElementChild.classList.remove(
      "icon-bell-silent"
    );
  }
  pushNotificationButton.disabled = false;
}

function checkIfSubscriber() {
  console.log("checkIfSubscriber");
  return new Promise((resolve, reject) => {
    DoJsonRequst("/api/subscribers/CheckIfPushNotificationSubscriber", {
      method: "POST",
      data: JSON.stringify({ EndPoint: pushNotificationSubscription.endpoint })
    })
      .then(response => {
        if (response.ok) return response.json();
      })
      .then(data => {
        return resolve(data.isSubscribed);
      })
      .catch(() => {
        reject();
      });
  });
}

function initializePushNotification() {
  pushNotificationButton.addEventListener("click", function() {
    console.log("notification clicked");
    if (isPushNotificationEnabled()) {
      Toast.toast("Are you Sure Want to UnSubscribe From Notification?", {
        onAccept: () => {
          DoJsonRequst("/api/subscribers/PushNotificationCancelSubscription", {
            method: "POST",
            data: JSON.stringify({
              EndPoint: pushNotificationSubscription.endpoint
            })
          })
            .then(res => {
              if (res.ok) {
                clearPushNotificationResult();
                Toast.toast("Unsubscribe Succeded  :)", { type: "success" });
              }
            })
            .catch(() =>
              Toast.toast("Ooops something went wrong! Try again later.", {
                type: "error"
              })
            )
            .finally(() => {
              updateNotificationButton();
            });
        },
        onReject: () => {
          updateNotificationButton();
        },
        sticky: true
      });
    } else {
      askPermission()
        .then(() => {
          subscribeUser()
            .then(response => {
              if (response.ok) {
                savePushNotificationResult();
              } else {
                Toast.toast("Ooops something went wrong :(", { type: "error" });
              }
            })
            .catch(() => {
              Toast.toast("Ooops something went wrong :(", { type: "error" });
            })
            .finally(() => {
              updateNotificationButton();
            });
        })
        .catch(error => {
          Toast.toast("Please Allow Notification from your browser settings.", {
            type: "warning",
            sticky: true
          });
        });
    }
  });
}

function subscribeUser() {
  return swRegisteration.pushManager
    .subscribe({
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
    })
    .then(sub => {
      let rawKey = sub.getKey ? sub.getKey("p256dh") : "";
      let key = rawKey
        ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawKey)))
        : "";
      let rawAuthSecret = sub.getKey ? sub.getKey("auth") : "";
      let authSecret = rawAuthSecret
        ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawAuthSecret)))
        : "";
      let endpoint = sub.endpoint;
      let pushNotificationSubscription = {
        Key: key,
        EndPoint: endpoint,
        AuthSecret: authSecret
      };
      return DoJsonRequst("/api/subscribers/PushNotificationSubscription", {
        method: "POST",
        data: JSON.stringify(pushNotificationSubscription)
      }).then(res => {
        return res;
      });
    })
    .then(result => {
      return result;
    });
}

function urlBase64ToUint8Array(base64String) {
  let padding = "=".repeat((4 - (base64String.length % 4)) % 4);
  let base64 = (base64String + padding).replace(/\-/g, "+").replace(/_/g, "/");
  let rawData = window.atob(base64);
  let outputArray = new Uint8Array(rawData.length);
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}

function DoJsonRequst(
  url,
  {
    method = "GET",
    headers = { "Content-Type": "application/json; charset=utf-8" },
    data
  }
) {
  return new Promise((resolve, reject) => {
    fetch(url, {
      method,
      headers,
      body: data
    })
      .then(response => resolve(response))
      .catch(error => reject(error));
  });
}

function isPushNotificationEnabled() {
  let isSubscribed =
    localStorage.getItem("PNSubscribedDate") &&
    new Date(localStorage.getItem("PNSubscribedDate")) > new Date();
  return isSubscribed;
}

function savePushNotificationResult() {
  let PNSubscribedDate = new Date();
  PNSubscribedDate.setDate(PNSubscribedDate.getDate() + 15);
  console.log(PNSubscribedDate);
  localStorage.setItem("PNSubscribedDate", PNSubscribedDate);
}

function clearPushNotificationResult() {
  localStorage.removeItem("PNSubscribedDate");
}

function askPermission() {
  return new Promise(function(resolve, reject) {
    const permissionResult = Notification.requestPermission(function(result) {
      resolve(result);
    });

    if (permissionResult) {
      permissionResult.then(resolve, reject);
    }
  }).then(function(permissionResult) {
    if (permissionResult !== "granted") {
      throw new Error("We weren't granted permission.");
    }
  });
}

function initializeAddToHomeScreen() {
  pwaInstallButton.addEventListener("click", () => {
    // Show the prompt
    deferredPrompt.prompt();
    // Wait for the user to respond to the prompt
    deferredPrompt.userChoice.then(function(choiceResult) {
      if (choiceResult.outcome === "accepted") {
        pwaInstallButton.style.display = "none";
      }
      // deferredPrompt = null;
    });
  });
}
