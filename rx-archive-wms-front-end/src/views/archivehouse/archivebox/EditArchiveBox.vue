<template>
  <BasicModal
    :title="t('编辑档案盒')"
    :width="600"
    :canFullscreen="false"
    @ok="submit"
    @register="registerModal"
    @visible-change="visibleChange"
    :destroyOnClose="true"
    :maskClosable="false"
  >
    <BasicForm @register="registerGoodsForm" />
  </BasicModal>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';
  import { BasicModal, useModalInner } from '/@/components/Modal';
  import { BasicForm, useForm } from '/@/components/Form/index';
  import { editFormSchema, updateStorageBoxAsync } from './ArchiveBox';
  import { CreateArchiveBoxDto } from '/@/services/ServiceProxies';
  import { useI18n } from '/@/hooks/web/useI18n';
  export default defineComponent({
    name: 'EditArchive',
    components: {
      BasicModal,
      BasicForm,
    },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerGoodsForm, { getFieldsValue, validate, setFieldsValue, resetFields }] =
        useForm({
          labelWidth: 120,
          schemas: editFormSchema,
          showActionButtonGroup: false,
        });
      const { t } = useI18n();
      let currentGoodsInfo = new CreateArchiveBoxDto();
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
        currentGoodsInfo = data.record;
        setFieldsValue({
          archiveBoxRfid: data.record.archiveBoxRfid,
          archiveBoxName: data.record.archiveBoxName,
          stockBarcode: data.record.stockBarcode,
          cellModel: data.record.cellModel,
          year: data.record.year,
          // secretLevel: [data.record.secretLevel],
          secretLevel: data.record.secretLevel === null ? null : data.record.secretLevel,
          retentionPeriod: data.record.retentionPeriod,
          catalogNo: data.record.catalogNo,
        });
      });

      const visibleChange = async (visible: boolean) => {
        if (visible) {
        } else {
        }
      };

      const submit = async () => {
        try {
          let request = getFieldsValue() as CreateArchiveBoxDto;
          request.id = currentGoodsInfo.id;
          await updateStorageBoxAsync({
            request: request,
            changeOkLoading,
            validate,
            closeModal,
            resetFields,
          });
          emit('reload');
        } catch (error) {
          changeOkLoading(false);
        }
      };
      const cancel = () => {
        resetFields();
        closeModal();
      };

      return {
        registerModal,
        registerGoodsForm,
        submit,
        visibleChange,
        cancel,
        t,
      };
    },
  });
</script>
<style lang="less" scoped>
  .ant-checkbox-wrapper + .ant-checkbox-wrapper {
    margin-left: 0;
  }
</style>
