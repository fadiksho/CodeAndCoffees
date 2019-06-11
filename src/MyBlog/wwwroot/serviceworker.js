var SITE_CACHE = 'site-cache-v2';
var Article_CACHE = 'article-cache-v2';

var offlineUrl = '/offline.html';

var urlsToCache = [
  // production
  './dist/css/site.css',
  './dist/css/enhancement.css',
  './dist/css/blog-detail.css',
  './dist/css/theme/theme-default.css',
  './dist/css/theme/theme-dark.css',
  './dist/css/theme/blog-detail-theme-dark.css',
  './dist/css/theme/blog-detail-theme-default.css',
  './dist/js/site.js',
  './dist/js/blog-detail.js',
  './images/auther-image.png',
  './offline.html',
  './manifest.json',
  './fonts/icons-font/icons-font.eot',
  './fonts/icons-font/icons-font.woff',
  './fonts/icons-font/icons-font.ttf'
];

self.addEventListener('install', function (event) {
  self.skipWaiting();
  // Perform install steps
  event.waitUntil(
    caches.open(SITE_CACHE)
      .then(function (cache) {
        return cache.addAll(urlsToCache);
      }).catch(function (e) {
        console.log(e);
      })
  );
});

self.addEventListener('fetch', function (event) {
  var request = event.request;

  // getting html from network then cache the offline html
  if (request.headers.get('Accept').indexOf('text/html') !== -1) {
    event.respondWith(getFromNetwork(request, 2000).then(
      function (response) {
        if (!response || response.status !== 200 || response.type !== 'basic') {
          return Promise.reject("couldn't connect to network");
        }
        addToCache(request, response, Article_CACHE);
        return response;
      }).catch(function () {
        return getFromCache(request, Article_CACHE).catch(function () {
          return getFromNetwork(request).then(function (response) {
            if (!response || response.status !== 200 || response.type !== 'basic') {
              return Promise.reject("couldn't connect to network");
            }
            addToCache(request, response, Article_CACHE);
            return response;
          }).catch(function () {
            return getOfflineHtml();
          });
        });
      })
    );
  }

  if (request.url.match(/\/*.(css|js|ttf|eot|woff)/ig)) {
    event.respondWith(
      caches.open(SITE_CACHE).then(function (cache) {
        return cache.match(request).then(function (matching) {
          return matching || fetch(request);
        });
      })
    );
  }

  if (request.url.match(/\/*.(png|gif|jpeg|svg)/ig)) {
    event.respondWith(
      caches.match(request)
        .then(function (response) {
          return response || fetch(request)
            .then(function (response) {
              if (!response || response.status !== 200 || response.type !== 'basic') {
                return response;
              }
              addToCache(request, response, Article_CACHE);
              return response;
            });
        })
    );
  }
});

self.addEventListener('activate', function (event) {
  console.log('activate');
  event.waitUntil(
    caches.keys()
      .then(function (keys) {
        // Remove caches whose name is no longer valid
        return Promise.all(keys
          .filter(function (key) {
            return [SITE_CACHE, Article_CACHE].indexOf(key) !== 0;
            //return key.indexOf(SITE_CACHE) !== 0;
          })
          .map(function (key) {
            return caches.delete(key);
          })
        );
      })
  );
});

self.addEventListener('push', function (event) {
  if (event.data) {
    var data = event.data.json();

    var payload = {
      body: data['Body'],
      icon: "/images/icons/homescreen512.png",
      badge: "/images/icons/homescreen192.png"
    };
    event.waitUntil(
      registration.showNotification(data['Title'], payload)
    );
  }
});

self.addEventListener('notificationclick', function (event) {
  event.notification.close();
  event.waitUntil(
    clients.matchAll({ type: 'window' }).then(windowClients => {
      // Check if there is already a window/tab open with the target URL
      for (var i = 0; i < windowClients.length; i++) {
        var client = windowClients[i];
        // If so, just focus it.
        console.log(client.url);
        if (client.url === '/' && 'focus' in client) {
          return client.focus();
        }
      }
      // If not, then open the target URL in a new window/tab.
      if (clients.openWindow) {
        return clients.openWindow('/');
      }
    })
  );
});


function addToCache(request, response, cacheName) {
  var copy = response.clone();
  caches.open(cacheName)
    .then(function (cache) {
      cache.put(request, copy);
    });
}

function getFromCache(request, cacheName) {
  return caches.open(cacheName).then(function (cache) {
    return cache.match(request).then(function (matching) {
      return matching || Promise.reject('no-match');
    });
  });
}

function getFromNetwork(request, timeout) {
  return new Promise(function (fulfill, reject) {
    if (timeout) {
      var timeoutId = setTimeout(reject, timeout);
      fetch(request).then(function (response) {
        clearTimeout(timeoutId);
        fulfill(response);
      }, reject);
    }
    else {
      fetch(request).then(function (response) {
        fulfill(response);
      }).catch(function () {
        reject();
      });
    }
  });
}

function getOfflineHtml() {
  return caches.open(SITE_CACHE).then(function (cache) {
    return cache.match(offlineUrl).then(function (matching) {
      return matching || Promise.reject('no-match');
    });
  });
}