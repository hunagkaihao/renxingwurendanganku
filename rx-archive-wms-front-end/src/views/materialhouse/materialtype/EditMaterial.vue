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
    import { defineComponent } from 'vue';
    import { BasicModal, useModalInner } from '/@/components/Modal';
    import { BasicForm, useForm } from '/@/components/Form/index';
    import { editFormSchema, updateMaterialAsync } from './MaterialType';
    import type { CreateMaterialDto, MaterialDto } from '/@/api/material';
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
        let currentMaterialInfo = {} as MaterialDto;
        const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
          currentMaterialInfo = data.record;
          setFieldsValue({
            materialCode: data.record.materialCode,
            materialName: data.record.materialName,
            materialType: data.record.materialType,
            materialUnit: data.record.materialUnit,
            validityDays: data.record.validityDays,
            creatorUserCode: data.record.creatorUserCode,
            materialCreateTime: data.record.materialCreateTime,
          });
        });
  
        const visibleChange = async (visible: boolean) => {
          if (visible) {
          } else {
          }
        };
  
        const submit = async () => {
          try {
            const request = getFieldsValue() as CreateMaterialDto;
            request.id = currentMaterialInfo.id;
            await updateMaterialAsync({
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
  
