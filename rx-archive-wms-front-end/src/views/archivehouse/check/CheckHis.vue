<template>
    <div>
      <BasicTable @register="registerTable"
        @selection-change="onSelectChange"
        :clickToRowSelect="true"
         size="small">
        <template #toolbar>
          <a-button
            type="primary"
            @click="checkComplete"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('完成计划') }}
          </a-button>
         
        </template>
        <template #isActive="{ record }">
          <Tag :color="record.isActive ? 'green' : 'red'">
            {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
          </Tag>
        </template>
        
      </BasicTable>
      <BasicTable @register="registerdetaildetailTable" size="small"
      @selection-change="onDetailSelectChange"
      :clickToRowSelect="true"
      >
        <template #toolbar>
          <a-button
            type="primary"
            @click="checkConfirm"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('账实一致确认') }}
          </a-button>
          <a-button
            type="primary"
            @click="checklossConfirm"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('盘亏确认') }}
          </a-button>
          <a-button
            type="primary"
            @click="checkIn"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('盘盈入库') }}
          </a-button>
          <a-button
            type="primary"
            @click="checkout"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('盘亏出库') }}
          </a-button>
          <!-- <a-button
            type="primary"
            @click="checkout"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('EXCEL导出') }}
          </a-button> -->
         
        </template>

      </BasicTable>
    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent,ref } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { hisColumns, checkhissearchFormSchema, hisDetailColumns, deleteGoodsAsync,GetTableDetailHis,GetTableHis,Confirm,LossConfirm,createSurplusIn,createLossOut,Complete } from './Check';
    import { useModal } from '/@/components/Modal';
    import { message } from 'ant-design-vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    export default defineComponent({
      name: 'CheckHis',
      components: {
        BasicTable,
        TableAction,
        Tag,
      },
      setup() {
        const { createConfirm } = useMessage();
        const { t } = useI18n();
        const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();
  
        const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();
  
        const [registerBlindBoxModal, { openModal: openBlindBoxModal }] = useModal();
  
        const [registerImportGoodssModal, { openModal: openImportGoodssModal }] = useModal();
        let selectedBoxIdRef = ref("")
        let selectedDetailRef = ref(0)
        var cellName ="";
        var boxRfid ="";
        var remark = "";
        var checkCode = "";
        // table配置
        const [registerTable, { reload }] = useTable({
          columns: hisColumns,
          formConfig: {
            labelWidth: 70,
            schemas: checkhissearchFormSchema,
          },
          api: GetTableHis,
          showTableSetting: true,
          useSearchForm: true,
          bordered: true,
          canResize: true,
          maxHeight:250,
          showIndexColumn: true,
          rowSelection: { type: 'radio' },
          
        });

        const [registerdetaildetailTable, {reload:reloadDetail}] = useTable({
          columns: hisDetailColumns,
          api: GetDetailHis,
          bordered: true,
          canResize: false,
          maxHeight:250,
          showIndexColumn: true,
          rowSelection: { type: 'radio' },
        });

        async function GetDetailHis(params){
            if (selectedBoxIdRef.value == "") {
            return {
              items: [],
              totalCount: 0,
            };
          }
          params.checkId = selectedBoxIdRef.value;
          return await GetTableDetailHis(params);
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
      //勾选事件
      const onSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedBoxIdRef.value = rows[0].id;
          checkCode = rows[0].checkCode;
        } else {
          selectedBoxIdRef.value = '';
        }
        reloadDetail();
      };
      const onDetailSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedDetailRef.value = rows[0].id;
          console.log(rows[0])
          cellName = rows[0].cellName;
          boxRfid = rows[0].stockBarcode;
          remark = rows[0].remark;
        } else {
          selectedDetailRef.value = 0;
          cellName = "";
          boxRfid = "";
          remark ="";
        }
      };

      async function checkComplete(){
        if(selectedBoxIdRef.value == ""){
            message.error("请先选择盘点计划")
            return
          }
          let msg = t('确认完成计划？');
          //let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await Complete(checkCode);
              },
          })
      }

      async function checkConfirm(){
        if(selectedDetailRef.value == 0){
            message.error("请先选择盘点结果")
            return
          }
          let msg = t('确认账实一致？');
          let id = selectedDetailRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await Confirm(id);
              },
          })
      }
      async function checklossConfirm(){
        if(selectedDetailRef.value == 0){
            message.error("请先选择盘点结果")
            return
          }
          let msg = t('确认盘亏处理？');
          let id = selectedDetailRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await LossConfirm(id);
              },
          })
      }
      async function checkIn(){
        if(selectedDetailRef.value == 0){
            message.error("请先选择盘点结果")
            return
          }
          let msg = t('确认盘盈入库？');
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await createSurplusIn(remark,cellName);
              },
          })
      }
      async function checkout(){
        if(selectedDetailRef.value == 0){
            message.error("请先选择盘点结果")
            return
          }
          let msg = t('确认盘亏出库？');
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await createLossOut(boxRfid,cellName);
              },
          })
      }
  
        return {
          registerTable,
          handleEdit,
          registerdetaildetailTable,
          handleDelete,
          onDetailSelectChange,
          registerCreateArchiveModal,
          openCreateArchiveModal,
          registerEditArchiveModal,
          registerBlindBoxModal,
          registerImportGoodssModal,
          openImportGoodssModal,
          handleBlindBox,
          t,
          reload,
          onSelectChange,
          checkComplete,
          checkConfirm,
          checklossConfirm,
          checkIn,
          checkout,
        };
      },
    });
  </script>
  
