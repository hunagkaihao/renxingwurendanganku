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
          @click="pickOut"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('借阅出库') }}
        </a-button>
        <a-button
          type="primary"
          @click="pickOut"
          v-auth="'WarehouseManagement.GoodsManagement.Create'"
        >
          {{ t('档案归还') }}
        </a-button>
        <a-button
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
        </a-button>
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
              auth: 'WarehouseManagement.GoodsManagement.Update',
              label: t('common.editText'),
              onClick: handleEdit.bind(null, record),
            },
          ]"
          :dropDownActions="[
            {
              auth: 'WarehouseManagement.GoodsManagement.Delete',
              label: t('common.delText'),
              onClick: handleDelete.bind(null, record),
            },
            {
              icon: 'eos-icons:cluster-role-binding',
              label: t('绑标签'),
              auth: 'WarehouseManagement.GoodsManagement.Update',
              onClick: handleBind.bind(null, record),
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
    <BindRfid @register="registerBindModal" @reload="reload" :bodyStyle="{ 'padding-top': '0' }" />
    <BlindBox
      @register="registerBlindBoxModal"
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
    deleteGoodsAsync,
    PickOut,
  } from './Archive';
  import { useModal } from '/@/components/Modal';
  import CreateArchive from './CreateArchive.vue';
  import EditArchive from './EditArchive.vue';
  import BindRfid from './BindRfid.vue';
  import BlindBox from './BlindBox.vue';
  import ImportGoodss from './ExcelImport.vue';
  import { message } from 'ant-design-vue';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { Tag } from 'ant-design-vue';
  import { PickOutDto } from '/@/services/ServiceProxies';
  export default defineComponent({
    name: 'Archive',
    components: {
      BasicTable,
      TableAction,
      CreateArchive,
      EditArchive,
      BlindBox,
      BindRfid,
      Tag,
      ImportGoodss,
    },
    setup() {
      const { createConfirm } = useMessage();
      const { t } = useI18n();
      const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();

      const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();
      const [registerBindModal, { openModal: openBindModal }] = useModal();
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
        rowKey: 'id',
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
        params.archiveId = selectedBoxIdRef.value;
        return await getDetaiTableListAsync(params);
      }

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
              await deleteGoodsAsync({ id: record.id, reload });
            },
          });
        }
      };

      async function pickOut() {
        var params = new Array<PickOutDto>();
        var p = new PickOutDto();
        p.archiveId = selectedBoxIdRef.value;
        params.push(p);
        await PickOut(params);
      }

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
        registerTable,
        registerDetailTable,
        reloadDetail,
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
        handleBind,
        registerBindModal,
        pickOut,
      };
    },
  });
</script>
