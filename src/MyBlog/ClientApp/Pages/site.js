import Toast from "../js/toast.js";

// get html document reference
var notificationButton = document.getElementById("subscribeButton");
var pwaInstallButton = document.getElementById("pwaInstallButton");

var isSubscribed = false;
var subscription = null;
var vapidPublicKey =
  "BDW5yAPVXetyXAN9C1F31REgmWRM9WlQUUX2r0ZgAPgqN_RlTKwN7eWJDHT0SOVpljZ8db3AyM4O91m4MMOuOTw";
var swRegistration = null;

var deferredPrompt = null;

// function subscribeUser() {
//   console.log("Trying To Subscribe Method Call");
//   swRegistration.pushManager
//     .subscribe({
//       userVisibleOnly: true,
//       applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
//     })
//     .then(function(sub) {
//       var rawKey = sub.getKey ? sub.getKey("p256dh") : "";
//       var key = rawKey
//         ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawKey)))
//         : "";
//       var rawAuthSecret = sub.getKey ? sub.getKey("auth") : "";
//       var authSecret = rawAuthSecret
//         ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawAuthSecret)))
//         : "";
//       var endpoint = sub.endpoint;
//       var pushNotificationSubscription = {
//         Key: key,
//         EndPoint: endpoint,
//         AuthSecret: authSecret
//       };
//       fetch("/api/subscribers/PushNotificationSubscription", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json; charset=utf-8"
//         },
//         body: JSON.stringify(pushNotificationSubscription)
//       })
//         .then(function(response) {
//           if (response.ok) {
//             isSubscribed = true;
//             subscription = sub;
//             updateNotificationButton();
//           } else {
//             toast("Ooops something went wrong :(", "error");
//           }
//         })
//         .catch(function() {
//           toast("Ooops something went wrong", "error");
//         });
//     });
// }
// function unsubscribeUser() {
//   fetch("/api/subscribers/PushNotificationCancelSubscription", {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json; charset=utf-8"
//     },
//     body: JSON.stringify({ EndPoint: subscription.endpoint })
//   })
//     .then(function(response) {
//       if (response.ok) {
//         subscription = null;
//         isSubscribed = false;
//         updateNotificationButton();
//         toast("Unsubscribe Succeded  :)", "succeed");
//       } else {
//         toast("Ooops something went wrong", "error");
//       }
//     })
//     .catch(function() {
//       toast("Ooops something went wrong", "error");
//     });
// }
// function initializeNotificationButton() {
//   notificationButton.style.display = "inline-block";

//   notificationButton.addEventListener("click", function() {
//     notificationButton.disabled = true;
//     if (isSubscribed) {
//       if (confirm("Are you Sure Want to UnSubscribe From Notification?")) {
//         unsubscribeUser();
//       } else {
//         notificationButton.disabled = false;
//       }
//     } else {
//       subscribeUser();
//     }
//   });
//   // Set the initial subscription value
//   swRegistration.pushManager.getSubscription().then(function(sub) {
//     if (sub) {
//       subscription = sub;
//       checkIfSubscriber();
//     } else {
//       updateNotificationButton();
//     }
//   });
// }
// function updateNotificationButton() {
//   if (Notification.permission === "denied") {
//     notificationButton.disabled = true;
//     return;
//   }
//   if (isSubscribed) {
//     notificationButton.classList.remove("icon-bell-slash");
//     notificationButton.classList.add("icon-bell");
//   } else {
//     notificationButton.classList.remove("icon-bell");
//     notificationButton.classList.add("icon-bell-slash");
//   }
//   notificationButton.disabled = false;
// }
// function checkIfSubscriber() {
//   console.log({ endPoint: subscription.endpoint });
//   notificationButton.disabled = true;
//   fetch("/api/subscribers/CheckIfPushNotificationSubscriber", {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json; charset=utf-8"
//     },
//     body: JSON.stringify({ EndPoint: subscription.endpoint })
//   })
//     .then(function(response) {
//       if (response.ok) {
//         return response.json();
//       }
//     })
//     .then(function(data) {
//       isSubscribed = data["isSubscribed"];
//       updateNotificationButton();
//     })
//     .catch(function() {
//       isSubscriber = false;
//       updateNotificationButton();
//     });
// }
// function urlBase64ToUint8Array(base64String) {
//   var padding = "=".repeat((4 - (base64String.length % 4)) % 4);
//   var base64 = (base64String + padding).replace(/\-/g, "+").replace(/_/g, "/");
//   var rawData = window.atob(base64);
//   var outputArray = new Uint8Array(rawData.length);
//   for (var i = 0; i < rawData.length; ++i) {
//     outputArray[i] = rawData.charCodeAt(i);
//   }
//   return outputArray;
// }

function addToHomeScreen() {
  // toast(
  //   "new debugging that should genereate a fucking sourse map another one",
  //   {
  //     onAccept: function() {
  //       console.log("Accepted!");
  //     },
  //     onReject: function() {
  //       console.log("Rejected!!!");
  //     }
  //   }
  // );
  Toast.toast({
    message: "New Update 1 Is Here",
    onAccept: function() {
      console.log("Accepting! 1");
    },
    type: "info",
    sticky: true
  });
  // if (deferredPrompt) {
  //   // Show the prompt
  //   deferredPrompt.prompt();
  //   // Wait for the user to respond to the prompt
  //   deferredPrompt.userChoice.then(function(choiceResult) {
  //     if (choiceResult.outcome === "accepted") {
  //       pwaInstallButton.style.display = "none";
  //     }
  //     deferredPrompt = null;
  //   });
  // }
}

window.addEventListener("load", function() {
  document
    .getElementById("pwaInstallButton")
    .addEventListener("click", () => addToHomeScreen());

  // Register ServiceWorker if Supported
  //#region
  // if ("serviceWorker" in navigator) {
  //   navigator.serviceWorker
  //     .register("/sw.js")
  //     .then(swReg => {
  //       swRegistration = swReg;
  //       console.log("SW registered.");
  //       if ("PushManager" in window) {
  //         console.log("Push Notification is supported.");
  //         initializeNotificationButton();
  //       }
  //     })
  //     .catch(swRegError => {
  //       console.log("SW registration failed: ", swRegError);
  //     });
  // }
  //#endregion

  // Check If We Are Runing From PWA APP
  if (
    window.matchMedia("(display-mode: standalone)").matches ||
    window.navigator.standalone === true
  ) {
    console.log("Check if we Are Running From PWA App");
    pwaInstallButton.style.display = "none";
  }
});

window.addEventListener("beforeinstallprompt", function(e) {
  // Prevent Chrome 67 and earlier from automatically showing the prompt
  console.log("BeforeInstallPromt");
  e.preventDefault();
  // Stash the event so it can be triggered later.
  deferredPrompt = e;
  // Update UI notify the user they can add to home screen
  pwaInstallButton.style.display = "inline-block";
});
window.addEventListener("appinstalled", function(evt) {
  pwaInstallButton.style.display = "none";
});
