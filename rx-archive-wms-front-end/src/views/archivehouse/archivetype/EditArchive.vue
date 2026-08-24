<template>
    <BasicModal
      :title="t('编辑档案盒')"
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
    import moment from 'moment'; //leixd
    import { defineComponent } from 'vue';
    import { BasicModal, useModalInner } from '/@/components/Modal';
    import { BasicForm, useForm } from '/@/components/Form/index';
    import { editFormSchema, updateGoodsAsync } from './Archivetype';
    import { UpdateGoodsDto, GoodsDto } from '/@/services/ServiceProxies';
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
        let currentGoodsInfo = new GoodsDto();
        const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
          currentGoodsInfo = data.record;
          setFieldsValue({
            goodsCode: data.record.goodsCode,
            goodsName: data.record.goodsSpec,
            goodsSpec: data.record.goodsSpec,
            goodsUnits: data.record.goodsSpec,
            goodsConstProperty1: data.record.goodsConstProperty1,
            shapeType: [data.record.shapeType],
            testdate: data.record.testdate,
            customer: data.record.customer,
          });
        });
  
        const visibleChange = async (visible: boolean) => {
          if (visible) {
          } else {
          }
        };
  
        const submit = async () => {
          try {
            let request = getFieldsValue() as UpdateGoodsDto;
            // let updateGoodsInput = new UpdateGoodsInput();
            request.id = currentGoodsInfo.id;
            request.testdate = moment(request.testdate).format();
            // request.publishDate = request.publishDate.format();
            await updateGoodsAsync({
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
  