const cacheName = "DefaultCompany-My project (1)-1.0";
const contentToCache = [
    "Build/out.loader.js",
    "Build/out.framework.js",
    "Build/out.data",
    "Build/out.wasm",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');

    e.waitUntil((async function () {
        const cache = await caches.open(cacheName);
        console.log('[Service Worker] Caching all: app shell and content');
        await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
        const filename = e.request.url.split('/').pop();
        console.log(`[Service Worker] filename: ${filename}`);

        let response;
        if (filename.endsWith('.bundle')) {
            response = await caches.match(filename);
        }
        else{
            response = await caches.match(e.request);
        }

        if (response) {
            console.log(`[Service Worker] キャッシュヒット: ${response.name}`);
            return response;
        }

        console.log(`[Service Worker] 通信取得: ${e.request.url}`);
        response = await fetch(e.request);
        const cache = await caches.open(cacheName);
        if (filename.endsWith('.bundle')) {
            console.log(`[Service Worker] キャッシュに代入: ${filename}`);
            cache.put(filename, response.clone());
        }
        else{
            console.log(`[Service Worker] キャッシュに代入: ${e.request.url}`);
            cache.put(e.request, response.clone());
        }
        return response;
    })());
});


// Service Workerスクリプト内
function IsInCached(inputString) {
    // ここでstringの評価を行い、boolの結果を返す
    if (inputString === 'example') {
        return true;
    } else {
        return false;
    }
}

self.addEventListener('message', event => {
    if (event.data && event.data.type === 'IsInCached') {
        const inputString = event.data.value;
        const response = await caches.match(inputString);
        self.clients.matchAll().then(clients => {
            clients.forEach(client => {
                client.postMessage({ type: 'result', value: response });
            });
        });
    }
});
