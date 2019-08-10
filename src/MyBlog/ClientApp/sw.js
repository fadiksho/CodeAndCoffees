workbox.core.skipWaiting();
workbox.core.clientsClaim();

// workbox.routing.registerRoute(/\.(?:js|css)$/);

self.addEventListener("push", event => {
  if (event.data) {
    var data = event.data.json();

    var payload = {
      body: data["Body"],
      icon: "/dist/icon.png",
      badge: "/dist/icon.png"
    };
    event.waitUntil(self.registration.showNotification(data["Title"], payload));
  }
});

workbox.precaching.precacheAndRoute(self.__precacheManifest);
