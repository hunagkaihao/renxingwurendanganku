// HTTP and SignalR share one origin; override at build time for another deployment.
// In a browser, use the host serving the Web UI so another client does not
// accidentally call its own loopback interface. Node-based tests keep the
// loopback fallback because they do not have a window location.
const defaultWcsApiUrl = typeof window !== 'undefined' && window.location && window.location.hostname
  ? `${window.location.protocol}//${window.location.hostname}:5200`
  : 'http://127.0.0.1:5200';

export const WCS_API_URL = (process.env.VUE_APP_WCS_API_URL || defaultWcsApiUrl).replace(/\/+$/, '');
export const WCS_HUB_URL = `${WCS_API_URL}/hub`;
