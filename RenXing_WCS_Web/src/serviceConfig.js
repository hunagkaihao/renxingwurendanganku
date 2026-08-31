// HTTP and SignalR share one origin; override at build time for another deployment.
export const WCS_API_URL = (process.env.VUE_APP_WCS_API_URL || 'http://127.0.0.1:5200').replace(/\/+$/, '');
export const WCS_HUB_URL = `${WCS_API_URL}/hub`;
