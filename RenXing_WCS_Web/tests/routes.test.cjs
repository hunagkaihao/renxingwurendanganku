const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const babel = require('@babel/core');
const compiler = require('vue-template-compiler');

// Run the actual component request builders; replace only UI imports and HTTP I/O.
function loadSource(relativePath) {
  const filename = path.resolve(__dirname, '..', relativePath);
  const text = fs.readFileSync(filename, 'utf8');
  const source = filename.endsWith('.vue') ? compiler.parseComponent(text).script.content : text;
  const code = babel.transformSync(source, {
    filename, babelrc: false, configFile: false,
    plugins: ['@babel/plugin-transform-modules-commonjs'],
  }).code;
  const module = { exports: {} };
  const localRequire = (name) => {
    if (name === 'element-ui' || name.endsWith('.css')) return {};
    if (name.startsWith('.')) {
      const resolved = path.resolve(path.dirname(filename), name);
      return loadSource(path.relative(path.resolve(__dirname, '..'), path.extname(resolved) ? resolved : `${resolved}.js`));
    }
    return require(name);
  };
  new Function('require', 'module', 'exports', code)(localRequire, module, module.exports);
  return module.exports;
}

const cases = [
  ['src/App.vue', 'pauseWcsServer', 'post', '/wcs/dispatch/core/pause'],
  ['src/App.vue', 'restartWcsServer', 'post', '/wcs/dispatch/core/restart'],
  ['src/App.vue', 'getWcsState', 'get', '/wcs/dispatch/core/wcsStatus'],
  ['src/App.vue', 'startTest', 'post', '/wcs/test/start'],
  ['src/App.vue', 'restartTest', 'post', '/wcs/test/restart'],
  ['src/App.vue', 'stopTest', 'post', '/wcs/test/stop'],
  ['src/pages/TagMonitorPage.vue', 'getPlcTagData', 'get', '/wcs/plc/plcMonitor'],
  ['src/pages/TagMonitorPage.vue', 'getMjjTagData', 'get', '/wcs/mjj/mjjStatusOfNmValMapList'],
  ['src/pages/OrderListMonitorPage.vue', 'getOrderData', 'get', '/wcs/dispatch/order/unDoneOrders'],
  ['src/pages/OrderListMonitorPage.vue', 'cancelOrder', 'post', '/wcs/dispatch/order/cancelOrder'],
  ['src/pages/OrderListMonitorPage.vue', 'forceDoneOrder', 'post', '/wcs/dispatch/order/forceDone'],
  ['src/pages/OneOrderMoniterPage.vue', 'updateOrder', 'get', '/wcs/dispatch/order/oneOrder'],
  ['src/pages/LogPage.vue', 'query', 'get', '/wcs/log/query'],
  ['src/components/BannerComp.vue', 'pauseWcsServer', 'post', '/wcs/dispatch/core/pause'],
  ['src/components/BannerComp.vue', 'restartWcsServer', 'post', '/wcs/dispatch/core/restart'],
  ['src/components/BannerComp.vue', 'getWcsState', 'get', '/wcs/dispatch/core/wcsStatus'],
];

for (const [file, method, verb, endpoint] of cases) {
  test(`${file}: ${method} targets ${verb.toUpperCase()} ${endpoint}`, () => {
    const component = loadSource(file).default;
    const calls = [];
    const context = {
      ...component.data(), select: 'ALL', input: '', maxCnt: 10,
      $route: { query: { orderCode: 'route-probe' } },
      $axios: (request) => { calls.push(request); return new Promise(() => {}); },
    };
    component.methods[method].call(context, { orderCode: 'route-probe' });
    assert.equal(calls.length, 1);
    assert.equal(calls[0].method, verb);
    const client = loadSource('src/axios.js').default;
    assert.equal(new URL(client.getUri(calls[0])).pathname, endpoint);
    if (['cancelOrder', 'forceDoneOrder'].includes(method))
      assert.deepEqual(calls[0].data, { orderCode: 'route-probe' });
    if (method === 'updateOrder') assert.equal(calls[0].params.orderCode, 'route-probe');
  });
}

test('monitor requests resolve to local WCS port 5200 by default', () => {
  const previous = process.env.VUE_APP_WCS_API_URL;
  delete process.env.VUE_APP_WCS_API_URL;
  try {
    assert.equal(loadSource('src/axios.js').default.getUri({ url: 'wcs/dispatch/order/states' }),
      'http://127.0.0.1:5200/wcs/dispatch/order/states');
  } finally {
    if (previous === undefined) delete process.env.VUE_APP_WCS_API_URL;
    else process.env.VUE_APP_WCS_API_URL = previous;
  }
});

test('deployment can override the WCS API origin without editing pages', () => {
  const previous = process.env.VUE_APP_WCS_API_URL;
  process.env.VUE_APP_WCS_API_URL = 'http://127.0.0.1:6200/';
  try {
    assert.equal(loadSource('src/axios.js').default.getUri({ url: 'wcs/dispatch/order/states' }),
      'http://127.0.0.1:6200/wcs/dispatch/order/states');
  } finally {
    if (previous === undefined) delete process.env.VUE_APP_WCS_API_URL;
    else process.env.VUE_APP_WCS_API_URL = previous;
  }
});

for (const origin of [undefined, 'http://127.0.0.1:6200/']) {
  test(`SignalR uses the same origin as HTTP (${origin || 'default'}) and keeps /hub`, () => {
    const signalR = require('@microsoft/signalr');
    const previous = process.env.VUE_APP_WCS_API_URL;
    const start = signalR.HubConnection.prototype.start;
    if (origin === undefined) delete process.env.VUE_APP_WCS_API_URL;
    else process.env.VUE_APP_WCS_API_URL = origin;
    // Keep the real URL builder/HubConnection; prevent only negotiation/network I/O.
    signalR.HubConnection.prototype.start = () => new Promise(() => {});
    try {
      const connection = loadSource('src/hubConnection.js').default;
      assert.equal(connection.baseUrl, origin ? 'http://127.0.0.1:6200/hub' : 'http://127.0.0.1:5200/hub');
    } finally {
      signalR.HubConnection.prototype.start = start;
      if (previous === undefined) delete process.env.VUE_APP_WCS_API_URL;
      else process.env.VUE_APP_WCS_API_URL = previous;
    }
  });
}
