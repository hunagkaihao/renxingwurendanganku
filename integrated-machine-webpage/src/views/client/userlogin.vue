<template>
    <div>
        <Modal
         class-name="terminal-login-modal"
         :width="620"
         :styles="{ top: '0', width: '620px', maxWidth: '100%', margin: '0 auto' }"
         :mask-closable="false"
         :closable="false"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <div slot="header" class="terminal-login-heading">
                <span class="terminal-login-symbol"><Icon type="ios-person-outline" size="32" /></span>
                <div><h2>用户登录</h2><p>请使用您的 WMS 账号继续操作</p></div>
            </div>
            <Form ref="userForm"  label-position="top" :rules="userRule" :model="loginModel">
                        <FormItem :label="L('用户名')" prop="userNameOrEmailAddress">
                            <Input ref="accountInput" v-model="loginModel.userNameOrEmailAddress" :disabled="submitting"
                              :maxlength="32" :minlength="2" placeholder="请输入用户名" autocomplete="username"
                              @on-focus="activeInput = 'accountInput'" @on-enter="focusPassword"></Input>
                        </FormItem>
                        <FormItem :label="L('密码')" prop="password">
                            <Input ref="passwordInput" v-model="loginModel.password" type="password" :disabled="submitting"
                              :maxlength="32" placeholder="请输入密码" autocomplete="current-password"
                              @on-focus="activeInput = 'passwordInput'" @on-enter="save"></Input>
                        </FormItem>        
            </Form>
            <div class="terminal-login-tools">
                <span><Icon type="ios-information-circle-outline" size="18" /> 点击输入框填写信息</span>
                <Button @click="openKeyboard" :disabled="submitting" icon="ios-keypad-outline">打开键盘</Button>
            </div>
            <p v-if="loginError" class="terminal-login-error" role="alert">{{ loginError }}</p>
            <div slot="footer" class="terminal-login-actions">
                <Button @click="cancel" :disabled="submitting">返回</Button>
                <Button @click="save" type="primary" :loading="submitting">{{ submitting ? '正在登录…' : '登录并继续' }}</Button>
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
        /** 登录请求期间锁定输入与重复提交，避免触屏连点。 */
        submitting:boolean = false;
        /** 当前登录失败信息，在弹窗内展示便于触屏用户处理。 */
        loginError:string = '';
        /** 唤起系统键盘前恢复到用户最近选择的输入框。 */
        activeInput:string = 'accountInput';
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
        /** 用户名输入完成后，将焦点移至密码框。 */
        focusPassword() {
            (this.$refs.passwordInput as any).focus();
        }
        /** 唤起一体机原有系统键盘；普通浏览器保留输入焦点并提示手动输入。 */
        openKeyboard()
        {
                (this.$refs[this.activeInput] as any).focus();
                try {
                    //打开键盘
                // @ts-ignore：无法被执行的代码的错误
                veinjs.openosk();
                } catch (error) {
                    this.$Message.info('请使用系统屏幕键盘或外接键盘输入');
                }
        }
        /** 验证表单并沿用现有 WMS 登录、会话初始化和业务页面跳转流程。 */
        save(){
            if (this.submitting) return;
            this.submitting = true;
            this.loginError = '';
            (this.$refs.userForm as any).validate(async (valid:boolean)=>{
                if (!valid) {
                    this.submitting = false;
                    return;
                }
                if(valid){
                  try {
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
                  } catch (error) {
                    this.loginError = '登录未完成，请检查账号密码及服务连接后重试。';
                  } finally {
                    this.submitting = false;
                  }
                }
            })
        }
        cancel(){
            if (this.submitting) return;
            this.loginError = '';
            (this.$refs.userForm as any).resetFields();
            this.$emit('input',false);
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }
            else
            {
                this.loginError = '';
                this.activeInput = 'accountInput';
                this.changeLanguage('zh-Hans');
            }
        };
        userRule={
            // userName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('UserName')),trigger: 'blur'}],
            // password:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('Password')),trigger: 'blur'}]
        }
    }
</script>
<style lang="less">
@import "./userlogin.less";
</style>

