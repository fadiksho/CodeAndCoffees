const SW_VERSION = "0.1.0";

workbox.setConfig({
  debug: false
});
workbox.precaching.cleanupOutdatedCaches();

const offlinePage = "/dist/offline.html";
// const notFoundPage = "/dist/404.html";

workbox.routing.registerRoute(
  new RegExp("/dist/pwa-icons/"),
  new workbox.strategies.StaleWhileRevalidate({
    cacheName: "pwa-icon-cache"
  })
);

// hanlde post pages
const postPagesMatchPlugin = {
  // request is the Request object that would otherwise be used as the cache key.
  // mode is either 'read' or 'write'.
  // Return either a string, or a Request whose url property will be used as the cache key.
  // Returning the original request will make this a no-op.
  cacheKeyWillBeUsed: async ({ request, mode }) => {
    let pathname = new URL(request.url).pathname;
    if (pathname === "/" || pathname === "/blog/page/1") {
      pathname = "/blog/page/1";
    }
    return pathname;
  }
};
const postPagesMatch = ({ url }) => {
  if (
    url.pathname === "/" ||
    new RegExp("/blog/page/[0-9]+").test(url.pathname)
  ) {
    return true;
  }
  return false;
};
const postPagesHandler = new workbox.strategies.NetworkFirst({
  cacheName: "blog-page-cache",
  plugins: [
    new workbox.cacheableResponse.Plugin({
      statuses: [200]
    }),
    new workbox.expiration.Plugin({
      maxEntries: 5
    }),
    postPagesMatchPlugin
  ]
});
workbox.routing.registerRoute(postPagesMatch, ({ event }) => {
  return postPagesHandler
    .handle({ event })
    .then(response => {
      return (
        response ||
        caches.match(workbox.precaching.getCacheKeyForURL(offlinePage))
      );
    })
    .catch(() => {
      return caches.match(workbox.precaching.getCacheKeyForURL(offlinePage));
    });
});

// handle post
const articleHandler = new workbox.strategies.NetworkFirst({
  cacheName: "post-cache",
  plugins: [
    new workbox.cacheableResponse.Plugin({
      statuses: [200]
    }),
    new workbox.expiration.Plugin({
      maxEntries: 50
    })
  ]
});
workbox.routing.registerRoute(new RegExp("/post/.+"), ({ event }) => {
  return articleHandler
    .handle({ event })
    .then(response => {
      return (
        response ||
        caches.match(workbox.precaching.getCacheKeyForURL(offlinePage))
      );
    })
    .catch(() => {
      return caches.match(workbox.precaching.getCacheKeyForURL(offlinePage));
    });
});

workbox.routing.registerRoute(
  /\.(?:png|gif|jpg|jpeg|svg)$/,
  new workbox.strategies.CacheFirst({
    cacheName: "images",
    plugins: [
      new workbox.expiration.Plugin({
        maxEntries: 60,
        maxAgeSeconds: 30 * 24 * 60 * 60 * 12 // 30 Days
      })
    ]
  })
);

workbox.routing.registerRoute(
  /\.(svg)$/,
  new workbox.strategies.CacheFirst({
    cacheName: "icon",
    plugins: [
      new workbox.expiration.Plugin({
        maxEntries: 60,
        maxAgeSeconds: 30 * 24 * 60 * 60 * 12 // 1 year
      })
    ]
  })
);

self.addEventListener("push", event => {
  if (event.data) {
    let data = event.data.json();
    let payload = {
      body: data.Body,
      icon: "/dist/images/icon.png",
      badge: "/dist/images/icon.png",
      data: data.Url
    };
    event.waitUntil(self.registration.showNotification(data["Title"], payload));
  }
});

self.addEventListener("message", event => {
  if (event.data && event.data.type === "SKIP_WAITING") {
    skipWaiting();
  }
});

self.addEventListener("notificationclick", function(event) {
  event.notification.close();
  event.waitUntil(
    clients.matchAll({ type: "window" }).then(windowClients => {
      // Check if there is already a window/tab open with the target URL
      for (var i = 0; i < windowClients.length; i++) {
        var client = windowClients[i];
        if (client.url === event.notification.data && "focus" in client) {
          return client.focus();
        }
      }
      // If not, then open the target URL in a new window/tab.
      if (clients.openWindow) {
        return clients.openWindow(event.notification.data);
      }
    })
  );
});
workbox.precaching.precacheAndRoute(self.__precacheManifest);
