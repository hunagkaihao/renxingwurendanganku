// The integrated-machine API is deployed alongside the web page by default.
// An explicit environment value is required when it is hosted elsewhere.
const configuredUrl = process.env.VUE_APP_REMOTE_SERVICE_BASE_URL;
const localApiUrl = typeof window === 'undefined'
  ? 'http://127.0.0.1:5000/'
  : `${window.location.protocol}//${window.location.hostname}:5000/`;

const URL = configuredUrl || localApiUrl;
export default URL;
