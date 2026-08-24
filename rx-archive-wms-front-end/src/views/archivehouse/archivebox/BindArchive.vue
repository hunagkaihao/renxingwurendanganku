<template>
  <BasicModal
    :title="t('绑定档案')"
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
  import { bindArchiveFormSchema, blindBoxAsync } from './ArchiveBox';
  import { GoodsDto, CreateArchiveBoxDto } from '/@/services/ServiceProxies';
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
          schemas: bindArchiveFormSchema,
          showActionButtonGroup: false,
        });
      const { t } = useI18n();
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
        setFieldsValue({
          archiveBoxRfid: data.record.archiveBoxRfid,
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
          console.log(request)
          await blindBoxAsync({
            request,
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
