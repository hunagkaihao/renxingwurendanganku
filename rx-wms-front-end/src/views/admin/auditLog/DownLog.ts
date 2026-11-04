import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import { useI18n } from '/@/hooks/web/useI18n';
const { t } = useI18n();
import moment from 'moment';
import { LogFilesServiceProxy, PagingAuditLogListInput } from '/@/services/ServiceProxies';
export const searchFormSchema: FormSchema[] = [
  {
    field: 'userName',
    label: t('routes.admin.userManagement_userName'),
    component: 'Input',
    colProps: { span: 8 },
  },
  {
    field: 'time',
    component: 'RangePicker',
    label: t('routes.admin.audit_executeTime'),
    colProps: {
      span: 6,
    },
  },
];

export const tableColumns: BasicColumn[] = [
  // {
  //   title: t('routes.admin.tenant'),
  //   dataIndex: 'tenantName',
  //   width: 100,
  // },
  {
    title: t('文件名'),
    dataIndex: 'logFileName',
    width: 200,
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    width: 200,
    
  },
  {
    title: '下载地址',
    dataIndex: 'logFileUrl',
    width: 350,
    slots:{ customRender: 'logFileUrl' },
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    width: 200,
    
  },
];


/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync() {
  const _auditLogsServiceProxy = new LogFilesServiceProxy();
  return _auditLogsServiceProxy.page();
}
