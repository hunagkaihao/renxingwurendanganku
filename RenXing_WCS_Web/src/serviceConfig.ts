const configured = import.meta.env.VITE_WCS_API_URL as string | undefined;
const defaultOrigin = typeof window !== 'undefined' && window.location.hostname
  ? `${window.location.protocol}//${window.location.hostname}:5200`
  : 'http://127.0.0.1:5200';

export const WCS_API_URL = (configured || defaultOrigin).replace(/\/+$/, '');
export const WCS_HUB_URL = `${WCS_API_URL}/hub`;
