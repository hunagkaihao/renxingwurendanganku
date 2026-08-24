<template>
  <BasicModal
    :title="t('routes.warehouse.goodsManagement_blindBox')"
    :width="600"
    :canFullscreen="false"
    @ok="submit"
    @cancel="cancel"
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
  import { blindBoxFormSchema, blindBoxAsync } from './Archive';
  import { CreateStorageDetailByBarcodeInput, GoodsDto } from '/@/services/ServiceProxies';
  import { useI18n } from '/@/hooks/web/useI18n';
  export default defineComponent({
    name: 'BlindBox',
    components: {
      BasicModal,
      BasicForm,
    },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerGoodsForm, { getFieldsValue, validate, setFieldsValue, resetFields }] =
        useForm({
          labelWidth: 120,
          schemas: blindBoxFormSchema,
          showActionButtonGroup: false,
        });
      const { t } = useI18n();
      let currentGoodsInfo = new GoodsDto();
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
        currentGoodsInfo = data.record;
        setFieldsValue({
          goodsCode: data.record.goodsCode,
          goodsName: data.record.goodsName,
          goodsSpec: data.record.goodsSpec,
          goodsConstProperty1: data.record.goodsConstProperty1,
          storageBoxBarCode: '',
          quantity: '1',
          goodsBatchNo: '',
        });
      });

      const visibleChange = async (visible: boolean) => {
        if (visible) {
        } else {
        }
      };

      const submit = async () => {
        try {
          let request = getFieldsValue() as CreateStorageDetailByBarcodeInput;
          // let updateGoodsInput = new UpdateGoodsInput();
          request.goodsId = currentGoodsInfo.id;
          // request.publishDate = request.publishDate.format();
          await blindBoxAsync({
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
    margin-left: 0px;
  }
</style>
