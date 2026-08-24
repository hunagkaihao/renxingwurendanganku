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
          type="primary"
          @click="createIn"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('入库') }}
        </a-button>
        <a-button
          type="primary"
          @click="createOut"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('出库') }}
        </a-button>
        <!-- <a-button
            type="primary"
            @click="openImportGoodssModal"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('EXCEL导入') }}
          </a-button>
          <a-button
            type="primary"
            @click="openImportGoodssModal"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('EXCEL导出') }}
          </a-button> -->
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
              icon: 'ant-design:edit-outlined',
              label: t('编辑'),
              auth: 'WarehouseManagement.GoodsManagement.Update',
              onClick: handleEdit.bind(null, record),
            },
          ]"
          :dropDownActions="[
            {
              auth: 'WarehouseManagement.GoodsManagement.Delete',
              icon: 'material-symbols:delete',
              label: t('删除'),
              onClick: handleDelete.bind(null, record),
            },
            {
              icon: 'eos-icons:cluster-role-binding',
              label: t('绑标签'),
              auth: 'WarehouseManagement.GoodsManagement.Update',
              onClick: handleBind.bind(null, record),
            },
            {
              icon: 'material-symbols:note-stack-add',
              label: t('绑档案'),
              auth: 'WarehouseManagement.GoodsManagement.Update',
              onClick: bindArchive.bind(null, record),
            },
          ]"
        />
      </template>
    </BasicTable>
    <a-row style="height: 50%; margin-left: 15px; margin-right: 15px">
      <BasicTable @register="registerDetailTable" size="small">
        <template #action="{ record }">
          <TableAction
            :actions="[
              {
                icon: 'ant-design:edit-outlined',
                auth: 'WarehouseManagement.GoodsManagement.Update',
                onClick: handleEdit.bind(null, record),
              },
            ]"
          />
        </template>
      </BasicTable>
    </a-row>
    <CreateArchive
      @register="registerCreateArchiveModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
    <EditArchive
      @register="registerEditArchiveModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
          <BindRfid
            @register="registerBindModal"
            @reload="reload"
            :bodyStyle="{ 'padding-top': '0' }"
          />
    <BindArchive
      @register="registerBindArchiveModal"
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
  import { defineComponent, ref } from 'vue';
  import { useMessage } from '/@/hooks/web/useMessage';
  import { BasicTable, useTable, TableAction } from '/@/components/Table';
  import {
    tableColumns,
    searchFormSchema,
    getTableListAsync,
    getDetaiTableListAsync,
    tableDetailColumns,
    deleteStorageBoxAsync,
    createWCSIn,
    createWCSOut,
  } from './ArchiveBox';
  import { useModal } from '/@/components/Modal';
  import CreateArchive from './CreateArchiveBox.vue';
  import EditArchive from './EditArchiveBox.vue';
  import BindArchive from './BindArchive.vue';
  import BindRfid from './BindRfid.vue';
  import ImportGoodss from './ExcelImport.vue';
  import { message } from 'ant-design-vue';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { Tag } from 'ant-design-vue';
  export default defineComponent({
    name: 'ArchiveBox',
    components: {
      BasicTable,
      TableAction,
      CreateArchive,
      EditArchive,
      BindRfid,
      BindArchive,
      Tag,
      ImportGoodss,
    },
    setup() {
      const { createConfirm } = useMessage();
      const { t } = useI18n();
      const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();

      const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();

      const [registerBindModal, { openModal: openBindModal }] = useModal();
      const [registerBindArchiveModal, { openModal: openBindArchiveModal }] = useModal();
      const [registerImportGoodssModal, { openModal: openImportGoodssModal }] = useModal();
      let selectedBoxIdRef = ref('');
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
        rowKey: 'id',
        clearSelectOnPageChange: true,
        maxHeight: 300,
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

      const [registerDetailTable, { reload: reloadDetail }] = useTable({
        columns: tableDetailColumns,
        api: getPageDetaiTableListAsync,
        showTableSetting: false,
        showIndexColumn: true,
        bordered: true,
        canResize: false,
        maxHeight: 300,
      });

      async function getPageDetaiTableListAsync(params) {
        if (selectedBoxIdRef.value == '') {
          return [];
        }
        params.archiveBoxId = selectedBoxIdRef.value;
        return await getDetaiTableListAsync(params);
      }

      // 编辑档案盒
      const handleEdit = (record: Recordable) => {
        openEditArchiveModal(true, {
          record: record,
        });
      };

      // 绑定标签
      const handleBind = (record: Recordable) => {
        openBindModal(true, {
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
          createConfirm({
            iconType: 'warning',
            title: t('common.tip'),
            content: msg,
            onOk: async () => {
              await deleteStorageBoxAsync({ id: record.id, reload });
            },
          });
        }
      };
      //绑定档案
      const bindArchive = async (record: Recordable) => {
        openBindArchiveModal(true, {
          record: record,
        });
      };
      //
      const createIn = async () => {
        if (selectedBoxIdRef.value == '') {
          message.error('请先选择档案盒');
          return;
        }
        let msg = t('确认入库？');
        let id = selectedBoxIdRef.value;
        createConfirm({
          iconType: 'warning',
          title: t('common.tip'),
          content: msg,
          onOk: async () => {
            await createWCSIn({ id, reload });
          },
        });
      };

      const createOut = async () => {
        if (selectedBoxIdRef.value == '') {
          message.error('请先选择档案盒');
          return;
        }
        let msg = t('确认出库？');
        let id = selectedBoxIdRef.value;
        createConfirm({
          iconType: 'warning',
          title: t('common.tip'),
          content: msg,
          onOk: async () => {
            await createWCSOut({ id, reload });
          },
        });
      };
      //勾选事件
      const onSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedBoxIdRef.value = rows[0].id;
        } else {
          selectedBoxIdRef.value = '';
        }
        reloadDetail();
      };

      return {
        onSelectChange,
        createIn,
        createOut,
        registerTable,
        handleEdit,
        handleDelete,
        registerCreateArchiveModal,
        openCreateArchiveModal,
        registerEditArchiveModal,
        registerBindModal,
        registerImportGoodssModal,
        openImportGoodssModal,
        registerDetailTable,
        registerBindArchiveModal,
        handleBind,
        bindArchive,
        t,
        reload,
      };
    },
  });
</script>
