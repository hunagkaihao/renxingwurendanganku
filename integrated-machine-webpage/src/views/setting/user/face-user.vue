<template>
    <div>
        <Modal
         :title="L('添加人脸验证')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="userForm"  label-position="top" :rules="userRule" :model="user">
                                          <FormItem :label="L('ID')" prop="id">
                            <Input  disabled="disabled"  v-model="user.id" :maxlength="32" :minlength="2"></Input>
                        </FormItem>
                          <FormItem :label="L('UserName')" prop="userName">
                            <Input  disabled="disabled"  v-model="user.userName" :maxlength="32" :minlength="2"></Input>
                        </FormItem>
                          <img ref="myimg"  id="myimg" :src="user.facePath">
                    <Upload
                        ref="upload"
                        :show-upload-list="false"
                        :default-file-list="defaultList"
                        :on-success="handleSuccess"
                        :format="['jpg','jpeg','png']"
                        :max-size="2048"
                        :on-format-error="handleFormatError"
                        :on-exceeded-size="handleMaxSize"
                        :before-upload="handleBeforeUpload"
                        type="drag"
                        action=""
                        style="display: inline-block;width:58px;">
                        <div style="width: 58px;height:58px;line-height: 58px;">
                            <Icon type="ios-camera" size="20"></Icon>
                        </div>
                    </Upload>
            </Form>
            <div slot="footer">
                <Button @click="cancel">{{L('Cancel')}}</Button>
                <Button @click="save" type="primary">{{L('OK')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import User from '../../../store/entities/user'
    import axios from 'axios'
    @Component
    export default class EditUserFace extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        user:User=new User();
         defaultList:any= [];
                imgName:string= '';
                visible:boolean= false;
                uploadList:any= [];
                file:any=null;
                imgUrl:any='';
        created(){
            
        }
        save(){
            (this.$refs.userForm as any).validate(async (valid:boolean)=>{
                if(valid){
                  let faceModel={
                    image:this.imgUrl.substr(this.imgUrl.search(";base64,")+8),
                    userId:this.user.id
                    }
                    // //baidu云上进行人脸识别
                    // await this.$store.dispatch({
                    //     type:'user/faceadd',
                    //     data:faceModel
                    // });


                                let rep =  axios.post('/UserManage', {
                                        "RequestValue":2817,
                                        "pass":"123456",
                                        "image_content": this.imgUrl.substr(this.imgUrl.search(";base64,")+8),
                                        "image_type": "image",
                                        "quality_control": "NONE",
                                         "user_id": this.user.id.toString(),
                                        "user_info": {
                                            "name":  this.user.id.toString()
                                        }


                                })
                            .then( async (response) => {
                                //人脸识别成功
                                if(response.data.ret_code==0)
                                {
                                    (this.$refs.userForm as any).resetFields();
                                    this.$emit('save-success');
                                    this.$emit('input',false);
                
                                }
                                else
                                {
                                    //人脸识别失败

                                    alert("人脸注册失败");
                                }
                                                // console.log(response.data.ret_code); 
                                console.log(response);
                            })
                            .catch(function (error) {
                                console.log(error);
                            });

                    let param = new FormData(); //创建form对象
                    param.append('file',this.file);//通过append向form对象添加数据
                    param.append('userId',this.user.id.toString());//通过append向form对象添加数据
                    param.append('photoType','face');//通过append向form对象添加数据
                    // console.log(param.get('file')); //FormData私有类对象，访问不到，可以通过get判断值是否传进去
                    // let config = {
                    // headers:{'Content-Type':'multipart/form-data'} //这里是重点，需要和后台沟通好请求头，Content-Type不一定是这个值
                    // }; //添加请求头
                    await this.$store.dispatch({
                        type:'user/imageUpload',
                        data:param
                    });                            
                    // (this.$refs.userForm as any).resetFields();
                    // this.$emit('save-success');
                    // this.$emit('input',false);
                }
            })
        }
        cancel(){
            (this.$refs.userForm as any).resetFields();
            this.$emit('input',false);
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                this.user=Util.extend(true,{},this.$store.state.user.editUser);
            }
        }

            handleSuccess (res, file) {
                //console.log(file.url);
                                //console.log(file.name);
               // file.url = 'https://o5wwk8baw.qnssl.com/7eb99afb9d5f317c912f08b5212fd69a/avatar';
                //file.name = '7eb99afb9d5f317c912f08b5212fd69a';
            }
            handleFormatError (file) {
                this.$Notice.warning({
                    title: 'The file format is incorrect',
                    desc: 'File format of ' + file.name + ' is incorrect, please select jpg or png.'
                });
            }
            handleMaxSize (file) {
                this.$Notice.warning({
                    title: 'Exceeding file size limit',
                    desc: 'File  ' + file.name + ' is too large, no more than 2M.'
                });
            }
            handleBeforeUpload (file) {
                this.file=file;
                const reader=new FileReader();
                //this.$Loading.start();
                reader.readAsDataURL(file);
                reader.onload=()=>{
                    const _base64=reader.result;
                    this.imgUrl=_base64;
                    console.log(_base64);
                }
                 let img=document.getElementById("myimg");
                 (this.$refs.myimg as any).src=this.imgUrl;
                 //img.attributes
                //this.$Loading.finish();
                return false;
                // const check = this.uploadList.length < 5;
                // if (!check) {
                //     this.$Notice.warning({
                //         title: 'Up to five pictures can be uploaded.'
                //     });
                // }
                // return check;
            }
            mounted () {
            this.uploadList = (this.$refs.upload as any).fileList;
        }

        userRule={
            userName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('UserName')),trigger: 'blur'}]
        }
    }
</script>
<style scoped  lang="less">
#myimg{
    width: 100%;
}
</style>