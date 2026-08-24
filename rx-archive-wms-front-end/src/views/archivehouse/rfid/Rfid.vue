<template>
  <div>
    <BasicTable
      @register="registerTable"
      @selection-change="onSelectChange"
      :clickToRowSelect="true"
      size="small"
    >
      <template #toolbar>
        <a-button
          preIcon="ant-design:plus-circle-outlined"
          type="primary"
          @click="openCreateArchiveModal"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('common.createText') }}
        </a-button>
        <a-button
          preIcon="ant-design:plus-circle-outlined"
          type="primary"
          @click="openImportGoodssModal"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('EXCEL导入') }}
        </a-button>
        <a-button type="primary" @click="jsonPrint">打印</a-button>
      </template>
      <template #isActive="{ record }">
        <Tag :color="record.isActive ? 'green' : 'red'">
          {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
        </Tag>
      </template>
      <template #action="{ record }">
        <TableAction
          :actions="[
            {
              auth: 'WarehouseManagement.GoodsManagement.Delete',
              label: t('common.delText'),
              onClick: handleDelete.bind(null, record),
            },
          ]"
          :dropDownActions="[]"
        />
      </template>
    </BasicTable>
    <CreateArchive
      @register="registerCreateArchiveModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
    <ImportGoodss
      @register="registerImportGoodssModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
  </div>
</template>

<script lang="ts">
  import { defineComponent, h, ref } from 'vue';
  import { useMessage } from '/@/hooks/web/useMessage';
  import { BasicTable, useTable, TableAction } from '/@/components/Table';
  import { tableColumns, searchFormSchema, getTableListAsync, deleteGoodsAsync } from './rfid';
  import { useModal } from '/@/components/Modal';
  import CreateArchive from './CreateArchive.vue';
  import ImportGoodss from './ExcelImport.vue';
  import { message, Modal } from 'ant-design-vue';
  import QRCode from 'qrcode';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { Tag } from 'ant-design-vue';
  import printJS from 'print-js';
  export default defineComponent({
    name: 'Archive',
    components: {
      BasicTable,
      TableAction,
      CreateArchive,
      Tag,
      ImportGoodss,
    },
    setup() {
      const { createConfirm } = useMessage();
      const { t } = useI18n();
      const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();

      const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();

      const [registerBlindBoxModal, { openModal: openBlindBoxModal }] = useModal();

      const [registerImportGoodssModal, { openModal: openImportGoodssModal }] = useModal();
      let selectedBoxIdRef = ref();

      // table配置
      const [registerTable, { reload }] = useTable({
        columns: tableColumns,
        formConfig: {
          labelWidth: 70,
          schemas: searchFormSchema,
        },
        api: getTableListAsync,
        showTableSetting: true,
        useSearchForm: true,
        bordered: true,
        canResize: true,
        showIndexColumn: true,
        rowSelection: { type: 'radio' },
        actionColumn: {
          width: 150,
          title: t('common.action'),
          dataIndex: 'action',
          slots: {
            customRender: 'action',
          },
          fixed: 'right',
        },
      });
      // 编辑用户
      const handleEdit = (record: Recordable) => {
        openEditArchiveModal(true, {
          record: record,
        });
      };

      // 绑定容器
      const handleBlindBox = (record: Recordable) => {
        openBlindBoxModal(true, {
          record: record,
        });
      };

      // 删除用户
      const handleDelete = async (record: Recordable) => {
        if (record.name == 'admin') {
          message.error('admin not delete');
          return;
        } else {
          let msg = t('common.askDelete');
          console.log(record);
          createConfirm({
            iconType: 'warning',
            title: t('common.tip'),
            content: msg,
            onOk: async () => {
              await deleteGoodsAsync({ id: record.id, reload });
            },
          });
        }
      };

      function jsonPrint() {
        // printJS({
        //   printable: [{ 借阅人: '李杰', 借出时间: '123@gmail.com', 电话: '123' }],
        //   properties: ['借阅人', '借出时间', '电话'],
        //   type: 'json',
        // });
        const boxId = selectedBoxIdRef.value;
        // 验证是否有值
        if (!boxId) {
          Modal.warning({
            title: '提示',
            content: '请先选择盒子',
          });
          return;
        }


        QRCode.toString(boxId.toString(), {
          type: 'svg',
          width: 280,
          margin: 2,
          errorCorrectionLevel: 'H',
        })
          .then((svgString) => {

            const printHtml = `
      <!DOCTYPE html>
      <html>
      <head>
        <meta charset="utf-8">
        <title>二维码 - ${boxId}</title>
        <style>
          body {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
            padding: 20px;
            font-family: Arial, sans-serif;
          }
          .qr-container {
            text-align: center;
            padding: 30px;
          }
          svg {
            width: 240px;
            height: 240px;
            margin: 20px auto;
            display: block;
          }
          .box-id {
            margin-top: 20px;
            font-size: 16px;
            color: #333;
          }
          @media print {
            body {
              margin: 0;
              padding: 0;
            }
          }
        </style>
      </head>
      <body>
        <div class="qr-container">
          <h3>档案盒标签</h3>
          ${svgString}
          <div class="box-id">编号: ${boxId}</div>
        </div>
      </body>
      </html>
    `;

            // 预览弹窗
            Modal.confirm({
              title: `盒子二维码 - ${boxId}`,
              width: 450,
              centered: true,
              content: h('div', { style: 'text-align: center; padding: 16px 0;' }, [
                h('div', {
                  style: 'width: 240px; height: 240px; margin: 0 auto;',
                  innerHTML: svgString,
                }),
                h(
                  'p',
                  { style: 'margin-top: 16px; font-size: 14px; color: #666;' },
                  `盒子编号: ${boxId}`
                ),
              ]),
              okText: '打印',
              cancelText: '取消',
              onOk: () => {
                // 方法1：使用 window.print 打印新窗口
                const printWindow = window.open('', '_blank');
                printWindow.document.write(printHtml);
                printWindow.document.close();
                printWindow.print();
              },
            });
          })
          .catch((error) => {
            console.error('生成二维码失败:', error);
            Modal.error({
              title: '生成失败',
              content: '二维码生成失败，请重试',
            });
          });
      }

      //勾选事件
      const onSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedBoxIdRef.value = rows[0].rfidCode;
        } else {
          selectedBoxIdRef.value = '';
        }
        console.log(selectedBoxIdRef.value);
      };

      return {
        onSelectChange,
        jsonPrint,
        registerTable,
        handleEdit,
        handleDelete,
        registerCreateArchiveModal,
        openCreateArchiveModal,
        registerEditArchiveModal,
        registerBlindBoxModal,
        registerImportGoodssModal,
        openImportGoodssModal,
        handleBlindBox,
        t,
        reload,
      };
    },
  });
</script>
