var notificationButton=document.getElementById("subscribeButton"),pwaInstallButton=document.getElementById("pwaInstallButton"),isSubscribed=!1,subscription=null,vapidPublicKey="BDW5yAPVXetyXAN9C1F31REgmWRM9WlQUUX2r0ZgAPgqN_RlTKwN7eWJDHT0SOVpljZ8db3AyM4O91m4MMOuOTw",swRegistration=null,EventSystem=function(){var e=document.createTextNode(null);this.addEventListener=e.addEventListener.bind(e),this.removeEventListener=e.removeEventListener.bind(e),this.dispatchEvent=e.dispatchEvent.bind(e)},userTheme=null;themeChangeEventTarget=new EventSystem;var themeChangeEvent=new Event("themeChangeEvent"),deferredPrompt=null;function addstyleSheetLink(e,t){var n=document.createElement("link");t&&n.setAttribute("id",t),n.setAttribute("rel","stylesheet"),n.setAttribute("type","text/css"),n.setAttribute("href",e),document.getElementsByTagName("head")[0].appendChild(n)}function removeStyleSheetLink(e){var t=document.getElementById(e);t&&t.parentNode.removeChild(t)}function styleExistById(e){return!!document.getElementById(e)}function getUserTheme(){var e=("; "+document.cookie).split("; theme-color=");if(2===e.length)return e.pop().split(";").shift()}function setUserTheme(e){var t=document.getElementById("toggleThemeButton");"default"===e?(t.classList.add("icon-sun-o"),t.classList.remove("icon-moon-o")):(t.classList.add("icon-moon-o"),t.classList.remove("icon-sun-o")),themeChangeEventTarget.dispatchEvent(themeChangeEvent)}function toggleTheme(){var e=document.querySelector("meta[name=theme-color]");userTheme&&"default"!==userTheme?(userTheme="default",e.setAttribute("content","#0F006B"),addstyleSheetLink("/dist/css/theme/theme-default.css","theme-default"),setTimeout(removeStyleSheetLink(removeStyleSheetLink("theme-dark"),2e3))):(userTheme="dark",e.setAttribute("content","#FF7B39"),addstyleSheetLink("/dist/css/theme/theme-dark.css","theme-dark"),setTimeout(removeStyleSheetLink("theme-default"),2e3)),setUserTheme(userTheme);var t=new Date;t.setTime(t.getTime()+31536e6),expires="; expires="+t.toUTCString(),document.cookie="theme-color="+(userTheme||"")+expires+"; path=/"}function subscribeUser(){console.log("Trying To Subscribe Method Call"),swRegistration.pushManager.subscribe({userVisibleOnly:!0,applicationServerKey:urlBase64ToUint8Array(vapidPublicKey)}).then(function(t){var e=t.getKey?t.getKey("p256dh"):"",n=e?btoa(String.fromCharCode.apply(null,new Uint8Array(e))):"",i=t.getKey?t.getKey("auth"):"",o=i?btoa(String.fromCharCode.apply(null,new Uint8Array(i))):"",s={Key:n,EndPoint:t.endpoint,AuthSecret:o};fetch("/api/subscribers/PushNotificationSubscription",{method:"POST",headers:{"Content-Type":"application/json; charset=utf-8"},body:JSON.stringify(s)}).then(function(e){e.ok?(isSubscribed=!0,subscription=t,updateNotificationButton()):toast("Ooops something went wrong :(","error")}).catch(function(){toast("Ooops something went wrong","error")})})}function unsubscribeUser(){fetch("/api/subscribers/PushNotificationCancelSubscription",{method:"POST",headers:{"Content-Type":"application/json; charset=utf-8"},body:JSON.stringify({EndPoint:subscription.endpoint})}).then(function(e){e.ok?(subscription=null,isSubscribed=!1,updateNotificationButton(),toast("Unsubscribe Succeded  :)","succeed")):toast("Ooops something went wrong","error")}).catch(function(){toast("Ooops something went wrong","error")})}function initializeNotificationButton(){notificationButton.style.display="inline-block",notificationButton.addEventListener("click",function(){notificationButton.disabled=!0,isSubscribed?confirm("Are you Sure Want to UnSubscribe From Notification?")?unsubscribeUser():notificationButton.disabled=!1:subscribeUser()}),swRegistration.pushManager.getSubscription().then(function(e){e?(subscription=e,checkIfSubscriber()):updateNotificationButton()})}function updateNotificationButton(){"denied"!==Notification.permission?(isSubscribed?(notificationButton.classList.remove("icon-bell-slash"),notificationButton.classList.add("icon-bell")):(notificationButton.classList.remove("icon-bell"),notificationButton.classList.add("icon-bell-slash")),notificationButton.disabled=!1):notificationButton.disabled=!0}function checkIfSubscriber(){console.log({endPoint:subscription.endpoint}),notificationButton.disabled=!0,fetch("/api/subscribers/CheckIfPushNotificationSubscriber",{method:"POST",headers:{"Content-Type":"application/json; charset=utf-8"},body:JSON.stringify({EndPoint:subscription.endpoint})}).then(function(e){if(e.ok)return e.json()}).then(function(e){isSubscribed=e.isSubscribed,updateNotificationButton()}).catch(function(){isSubscriber=!1,updateNotificationButton()})}function urlBase64ToUint8Array(e){for(var t=(e+"=".repeat((4-e.length%4)%4)).replace(/\-/g,"+").replace(/_/g,"/"),n=window.atob(t),i=new Uint8Array(n.length),o=0;o<n.length;++o)i[o]=n.charCodeAt(o);return i}function toast(e,t){var n=document.getElementById("snackbar");switch(t){case"error":n.style.backgroundColor="red";break;case"succeed":n.style.backgroundColor="green";break;default:n.style.backgroundColor="#0f006b"}n.className="show",n.innerText=e,setTimeout(function(){n.className=n.className.replace("show","")},4e3)}function addToHomeScreen(){deferredPrompt&&(deferredPrompt.prompt(),deferredPrompt.userChoice.then(function(e){"accepted"===e.outcome&&(pwaInstallButton.style.display="none"),deferredPrompt=null}))}window.addEventListener("load",function(){(userTheme=getUserTheme())&&("default"===userTheme?styleExistById("theme-default")||(addstyleSheetLink("/dist/css/theme/theme-default.css","theme-default"),removeStyleSheetLink("theme-dark"),setUserTheme("default")):styleExistById("theme-dark")||(addstyleSheetLink("/dist/css/theme/theme-dark.css","theme-dark"),removeStyleSheetLink("theme-default"),setUserTheme("dark"))),"serviceWorker"in navigator&&navigator.serviceWorker.register("/serviceworker.js",{updateViaCache:"none"}).then(function(e){swRegistration=e,"PushManager"in window&&initializeNotificationButton()}).catch(function(e){console.log("ServiceWorker registration failed: ",e)}),!window.matchMedia("(display-mode: standalone)").matches&&!0!==window.navigator.standalone||(console.log("Check if we Are Running From PWA App"),pwaInstallButton.style.display="none")}),window.addEventListener("online",function(){updateNotificationButton()}),window.addEventListener("offline",function(){notificationButton.disabled=!0}),window.addEventListener("beforeinstallprompt",function(e){console.log("BeforeInstallPromt"),e.preventDefault(),deferredPrompt=e,pwaInstallButton.style.display="inline-block"}),window.addEventListener("appinstalled",function(e){pwaInstallButton.style.display="none"});