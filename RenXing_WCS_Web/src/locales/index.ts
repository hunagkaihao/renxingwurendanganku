import { createI18n } from 'vue-i18n';

export const i18n = createI18n({ legacy: false, locale: 'zh-CN', fallbackLocale: 'en-US', messages: {
  'zh-CN': { menu: { orders: '订单监控', tags: '点位监控', logs: '日志查询' }, state: { Running: 'WCS 服务执行中', Pause: 'WCS 服务暂停中', Unknown: 'WCS 服务状态未知' } },
  'en-US': { menu: { orders: 'Orders', tags: 'Tags', logs: 'Logs' }, state: { Running: 'WCS Running', Pause: 'WCS Paused', Unknown: 'WCS Unknown' } }
} });
