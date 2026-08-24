<template>
  <BasicModal
    :title="t('routes.material.archiveManagement_edit_archives')"
    :width="600"
    :canFullscreen="false"
    @ok="submit"
    @cancel="cancel"
    @register="registerModal"
    @visible-change="visibleChange"
    :destroyOnClose="true"
    :maskClosable="false"
  >
    <BasicForm @register="registerArchivesForm" />
  </BasicModal>
</template>

<script lang="ts">
  import moment from 'moment'; //leixd
  import { defineComponent } from 'vue';
  import { BasicModal, useModalInner } from '/@/components/Modal';
  import { BasicForm, useForm } from '/@/components/Form/index';
  import { editFormSchema, updateArchivesAsync } from './Archive';
  import { CreateArchiveDto, ArchiveDto } from '/@/services/ServiceProxies';
  import { useI18n } from '/@/hooks/web/useI18n';
  export default defineComponent({
    name: 'EditArchive',
    components: {
      BasicModal,
      BasicForm,
    },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerArchivesForm, { getFieldsValue, validate, setFieldsValue, resetFields }] =
        useForm({
          labelWidth: 120,
          schemas: editFormSchema,
          showActionButtonGroup: false,
        });
      const { t } = useI18n();
      let currentArchivesInfo = new ArchiveDto();
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner((data) => {
        currentArchivesInfo = data.record;
        setFieldsValue({
          archivesRfid: data.record.rfidId,
          archivesCode: data.record.archivesCode,
          archivesName: data.record.archivesName,
          year: data.record.year,
          secretLevel: data.record.secretLevel,
          classType: data.record.classType,
          retentionPeriod: data.record.retentionPeriod,
        });
      });

      const visibleChange = async (visible: boolean) => {
        if (visible) {
        } else {
        }
      };

      const submit = async () => {
        try {
          let request = getFieldsValue() as CreateArchiveDto;
          request.id = currentArchivesInfo.id;
          request.rfidId = request.archivesRfid;
          await updateArchivesAsync({
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
        registerArchivesForm,
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
