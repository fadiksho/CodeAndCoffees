// // workbox.core.skipWaiting();
// // workbox.core.clientsClaim();

workbox.precaching.cleanupOutdatedCaches();

const offlinePage = "/dist/offline.html";
const notFoundPage = "/dist/404.html";

workbox.routing.registerRoute(
  new RegExp("/dist/icons/"),
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
      if (!response) {
        return caches.match(workbox.precaching.getCacheKeyForURL(offlinePage));
      } else if (response.status === 404) {
        return caches.match(workbox.precaching.getCacheKeyForURL(notFoundPage));
      }
      return response;
    })
    .catch(() => {
      return response;
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
      if (!response) {
        return caches.match(workbox.precaching.getCacheKeyForURL(offlinePage));
      } else if (response.status === 404) {
        return caches.match(workbox.precaching.getCacheKeyForURL(notFoundPage));
      }
      return response;
    })
    .catch(() => {
      return response;
    });
});

workbox.routing.registerRoute(
  /\.(?:png|gif|jpg|jpeg|svg)$/,
  new workbox.strategies.CacheFirst({
    cacheName: "images",
    plugins: [
      new workbox.expiration.Plugin({
        maxEntries: 60,
        maxAgeSeconds: 30 * 24 * 60 * 60 // 30 Days
      })
    ]
  })
);

workbox.routing.setCatchHandler(({ url, event, params }) => {
  console.log(`Catch Error \n ${url} \n ${event} \n ${params}`);
});

addEventListener("push", event => {
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

addEventListener("message", event => {
  if (event.data && event.data.type === "SKIP_WAITING") {
    console.log("new service worker update to new version trying to update!!!");
    skipWaiting();
  }
});

workbox.precaching.precacheAndRoute(self.__precacheManifest);
