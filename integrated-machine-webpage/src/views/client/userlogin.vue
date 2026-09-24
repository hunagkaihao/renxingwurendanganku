<template>
    <div>
        <Modal
         :title="L('用户登录')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="userForm"  label-position="top" :rules="userRule" :model="loginModel">
                        <FormItem :label="L('用户名')" prop="userName">
                            <Input v-model="loginModel.userNameOrEmailAddress" :maxlength="32" :minlength="2"></Input>
                        </FormItem>
                        <FormItem :label="L('密码')" prop="password">
                            <Input v-model="loginModel.password" type="password" :maxlength="32">  <Icon type="ios-keypad" slot="suffix"  @click="openKeyboard" />  </Input>
                        </FormItem>        
            </Form>
            <div slot="footer">
                <Button @click="cancel">{{L('取消')}}</Button>
                <Button @click="save" type="primary">{{L('确认')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../lib/util'
    import AbpBase from '../../lib/abpbase'
    import Cookies from 'js-cookie';
    @Component
    export default class UserLogin extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
            loginModel={
                userNameOrEmailAddress:'admin',
                password:'',
                rememberMe:false
            }
        changeLanguage(languageName:string){
            abp.utils.setCookieValue(
                "Abp.Localization.CultureName",
                languageName,
                new Date(new Date().getTime() + 5 * 365 * 86400000), //5 year
                abp.appPath
            );
            // location.reload();
        }
        openKeyboard()
        {
                try {
                    console.log("openkeyboard");
                    //打开键盘
                // @ts-ignore：无法被执行的代码的错误
                veinjs.openosk();
                } catch (error) {}
        }
        save(){
            (this.$refs.userForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    // this.$Message.loading({
                    //     content:this.L('LoginPrompt'),
                    //     duration:0
                    // })
                    await this.$store.dispatch({
                        type:'app/login',
                        data:this.loginModel
                    })
                    sessionStorage.setItem('rememberMe','0');
                       await this.$store.dispatch({
                            type:'session/init'
                        })
                        console.log(this.$store.state.session.user.id);
                        console.log(this.$store.state.session.user.name);
                        Cookies.set('facing', '0');
                        Cookies.set('userId', this.$store.state.session.user.id);
                        //location.reload();
                        this.$router.push({
                            name: Cookies.get('last_page_name')
                        });
                    //location.reload();
                    (this.$refs.userForm as any).resetFields();
                    this.$emit('save-success');
                    this.$emit('input',false);
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
            }
            else
            {
                this.changeLanguage('zh-Hans');
            }
        };
        userRule={
            // userName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('UserName')),trigger: 'blur'}],
            // password:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('Password')),trigger: 'blur'}]
        }
    }
</script>

