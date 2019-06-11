// get html document reference
var notificationButton = document.getElementById('subscribeButton');
var pwaInstallButton = document.getElementById('pwaInstallButton');

var isSubscribed = false;
var subscription = null;
var vapidPublicKey = 'BDW5yAPVXetyXAN9C1F31REgmWRM9WlQUUX2r0ZgAPgqN_RlTKwN7eWJDHT0SOVpljZ8db3AyM4O91m4MMOuOTw';
var swRegistration = null;
var EventSystem = function () {
  var target = document.createTextNode(null);
  this.addEventListener = target.addEventListener.bind(target);
  this.removeEventListener = target.removeEventListener.bind(target);
  this.dispatchEvent = target.dispatchEvent.bind(target);
};
var userTheme = null;
themeChangeEventTarget = new EventSystem();
var themeChangeEvent = new Event('themeChangeEvent');
var deferredPrompt = null;

function addstyleSheetLink(filePath, id) {
  var themeSheet = document.createElement("link");
  if (id) {
    themeSheet.setAttribute("id", id);
  }
  themeSheet.setAttribute("rel", "stylesheet");
  themeSheet.setAttribute("type", "text/css");
  themeSheet.setAttribute("href", filePath);
  document.getElementsByTagName("head")[0].appendChild(themeSheet);
}
function removeStyleSheetLink(id) {
  var style = document.getElementById(id);
  if (style) {
    style.parentNode.removeChild(style);
  }
}
function styleExistById(id) {
  if (document.getElementById(id)) {
    return true;
  }
  return false;
}
function getUserTheme() {
  var value = "; " + document.cookie;
  var parts = value.split("; " + "theme-color" + "=");
  if (parts.length === 2) return parts.pop().split(";").shift();
}
function setUserTheme(theme) {
  var toggleThemeButton = document.getElementById('toggleThemeButton');
  if (theme === 'default') {
    toggleThemeButton.classList.add("icon-sun-o");
    toggleThemeButton.classList.remove("icon-moon-o");
  } else {
    toggleThemeButton.classList.add("icon-moon-o");
    toggleThemeButton.classList.remove("icon-sun-o");
  }
  themeChangeEventTarget.dispatchEvent(themeChangeEvent);
}
function toggleTheme() {
  var metaThemeColor = document.querySelector("meta[name=theme-color]");
  if (!userTheme || userTheme === 'default') {
    userTheme = 'dark';
    metaThemeColor.setAttribute("content", '#FF7B39');
    addstyleSheetLink('/dist/css/theme/theme-dark.css', 'theme-dark');
    setTimeout(removeStyleSheetLink('theme-default'), 2000);

  } else {
    userTheme = 'default';
    metaThemeColor.setAttribute("content", '#0F006B');
    addstyleSheetLink('/dist/css/theme/theme-default.css', 'theme-default');
    setTimeout(removeStyleSheetLink(removeStyleSheetLink('theme-dark'), 2000));
  }

  setUserTheme(userTheme);

  // save user theme in cookie
  var date = new Date();
  date.setTime(date.getTime() + 365 * 24 * 60 * 60 * 1000);
  expires = "; expires=" + date.toUTCString();
  document.cookie = 'theme-color' + "=" + (userTheme || "") + expires + "; path=/";
}

function subscribeUser() {
  console.log('Trying To Subscribe Method Call');
  swRegistration.pushManager.subscribe({
    userVisibleOnly: true,
    applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
  })
    .then(function (sub) {
      var rawKey = sub.getKey ? sub.getKey('p256dh') : '';
      var key = rawKey ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawKey))) : '';
      var rawAuthSecret = sub.getKey ? sub.getKey('auth') : '';
      var authSecret = rawAuthSecret ? btoa(String.fromCharCode.apply(null, new Uint8Array(rawAuthSecret))) : '';
      var endpoint = sub.endpoint;
      var pushNotificationSubscription = {
        Key: key,
        EndPoint: endpoint,
        AuthSecret: authSecret
      };
      fetch('/api/subscribers/PushNotificationSubscription', {
        method: 'POST',
        headers: {
          "Content-Type": "application/json; charset=utf-8"
        },
        body: JSON.stringify(pushNotificationSubscription)
      }).then(function (response) {
        if (response.ok) {
          isSubscribed = true;
          subscription = sub;
          updateNotificationButton();
        }
        else {
          toast('Ooops something went wrong :(', 'error');
        }
        }).catch(function () {
          toast('Ooops something went wrong', 'error');
        });
    });
}
function unsubscribeUser() {
  fetch('/api/subscribers/PushNotificationCancelSubscription', {
    method: 'POST',
    headers: {
      "Content-Type": "application/json; charset=utf-8"
    },
    body: JSON.stringify({
      EndPoint: subscription.endpoint
    })
  }).then(function (response) {
    if (response.ok) {
      subscription = null;
      isSubscribed = false;
      updateNotificationButton();
      toast('Unsubscribe Succeded  :)', 'succeed');
    }
    else {
      toast('Ooops something went wrong', 'error');
    }
  }).catch(function () {
    toast('Ooops something went wrong', 'error');
  });
}
function initializeNotificationButton() {
  notificationButton.style.display = 'inline-block';
  
  notificationButton.addEventListener("click", function () {
    notificationButton.disabled = true;
    if (isSubscribed) {
      if (confirm("Are you Sure Want to UnSubscribe From Notification?")) {
        unsubscribeUser();
      } else {
        notificationButton.disabled = false;
      }
    } else {
      subscribeUser();
    }
  });
  // Set the initial subscription value
  swRegistration.pushManager.getSubscription()
    .then(function (sub) {
      if (sub) {
        subscription = sub;
        checkIfSubscriber();
      } else {
        updateNotificationButton();
      }
    });
}
function updateNotificationButton() {
  if (Notification.permission === 'denied') {
    notificationButton.disabled = true;
    return;
  }
  if (isSubscribed) {
    notificationButton.classList.remove('icon-bell-slash');
    notificationButton.classList.add('icon-bell');
  }
  else {
    notificationButton.classList.remove('icon-bell');
    notificationButton.classList.add('icon-bell-slash');
  }
  notificationButton.disabled = false;
}
function checkIfSubscriber() {
  notificationButton.disabled = true;
  fetch('/api/subscribers/CheckIfPushNotificationSubscriber', {
    method: 'POST',
    headers: {
      "Content-Type": "application/json; charset=utf-8"
    },
    body: JSON.stringify(subscription)
  }).then(function (response) {
    if (response.ok) {
      return response.json();
    }
  }).then(function (data) {
    isSubscribed = data["isSubscribed"];
    updateNotificationButton();
  }).catch(function () {
    isSubscriber = false;
    updateNotificationButton();
  });

}
function urlBase64ToUint8Array(base64String) {
  var padding = '='.repeat((4 - base64String.length % 4) % 4);
  var base64 = (base64String + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');
  var rawData = window.atob(base64);
  var outputArray = new Uint8Array(rawData.length);
  for (var i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}

function toast(message, type) {
  // Get the snackbar DIV
  var snackbar = document.getElementById("snackbar");
  switch (type) {
    case 'error':
      snackbar.style.backgroundColor = "red";
      break;
    case 'succeed':
      snackbar.style.backgroundColor = 'green';
      break;
    default:
      snackbar.style.backgroundColor = '#0f006b';
  }
  // Add the "show" class to DIV
  snackbar.className = "show";
  snackbar.innerText = message;
  // After 3 seconds, remove the show class from DIV
  setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, 4000);
}

function addToHomeScreen() {
  if (deferredPrompt) {
    // Show the prompt
    deferredPrompt.prompt();
    // Wait for the user to respond to the prompt
    deferredPrompt.userChoice
      .then(function (choiceResult) {
        if (choiceResult.outcome === 'accepted') {
          pwaInstallButton.style.display = 'none';
        }
        deferredPrompt = null;
      });
  }
}

window.addEventListener('load', function () {
  // Update Theme
  userTheme = getUserTheme();
  if (userTheme) {
    if (userTheme === 'default') {
      if (!styleExistById('theme-default')) {
        addstyleSheetLink('/dist/css/theme/theme-default.css', 'theme-default');
        removeStyleSheetLink('theme-dark');
        setUserTheme('default');
      }
    } else {
      if (!styleExistById('theme-dark')) {
        addstyleSheetLink('/dist/css/theme/theme-dark.css', 'theme-dark');
        removeStyleSheetLink('theme-default');
        setUserTheme('dark');
      }
    }
  }
  // Register ServiceWorker if Supported
  if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/serviceworker.js', {
      updateViaCache: 'none'
    }).then(function (swReg) {
      swRegistration = swReg;
      if ('PushManager' in window) {
        initializeNotificationButton();
      }
    }).catch(function (error) {
      console.log('ServiceWorker registration failed: ', error);
    });
  }
  // Check If We Are Runing From PWA APP
  if (window.matchMedia('(display-mode: standalone)').matches || window.navigator.standalone === true) {
    console.log('Check if we Are Running From PWA App');
    pwaInstallButton.style.display = 'none';
  }
});
window.addEventListener('online', function () {
  updateNotificationButton();
});
window.addEventListener('offline', function () {
  notificationButton.disabled = true;
});
window.addEventListener('beforeinstallprompt', function (e) {
  // Prevent Chrome 67 and earlier from automatically showing the prompt
  console.log('BeforeInstallPromt');
  e.preventDefault();
  // Stash the event so it can be triggered later.
  deferredPrompt = e;
  // Update UI notify the user they can add to home screen
  pwaInstallButton.style.display = 'inline-block';
});
window.addEventListener('appinstalled', function (evt) {
  pwaInstallButton.style.display = 'none';
});