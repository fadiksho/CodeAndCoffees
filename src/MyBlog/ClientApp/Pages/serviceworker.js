import { Workbox } from "workbox-window";
import Toast from "../js/toast";
console.log("Running Service Worker JS!");
if ("serviceWorker" in navigator) {
  const wb = new Workbox("/sw.js");

  wb.addEventListener("activated", event => {
    // `event.isUpdate` will be true if another version of the service
    // worker was controlling the page when this version was registered.
  });

  wb.addEventListener("waiting", event => {
    Toast.toast({
      message: "New Version Is here Update now",
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
  wb.register();
}
