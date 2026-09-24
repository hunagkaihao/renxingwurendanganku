<template>
</template>
<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import AbpBase from '../../lib/abpbase'
import Util from '../../lib/util';
import appconst from '../../lib/appconst'
import Cookies from 'js-cookie';
import axios from 'axios'
@Component
export default class autol extends AbpBase {
      //使用AI镜头
       async  validator1 () {
        await axios.post("http://192.168.1.188:21021/api/TokenAuth/AIFaceLogin",
            {
                 userid:0,
                 client:"Client1",
                 TimeInterval:35
            }
        ).then( async (response) => {
            var tokenExpireDate = false ? (new Date(new Date().getTime() + 1000 * response.data.result.expireInSeconds)) : undefined;
            Util.abp.auth.setToken(response.data.result.accessToken, tokenExpireDate);
            abp.utils.setCookieValue('Abp.Localization.CultureName','zh-Hans',new Date(new Date().getTime() + 5 * 365 * 86400000),Util.abp.appPath);
            Util.abp.utils.setCookieValue(appconst.authorization.encrptedAuthTokenName, response.data.result.encryptedAccessToken, tokenExpireDate, Util.abp.appPath)
                            await this.$store.dispatch({
                    type:'session/init'
                })
                    Cookies.set('facing', '0');
                    Cookies.set('userId', this.$store.state.session.user.id);
                    //location.reload();
                    this.$router.push({
                        name: Cookies.get('last_page_name')
                    });
                        return true;
              })
              .catch( (error) => {
                // //20220420
                alert('用户未启用');//报错或者用户未启用  或者后台未在登录记录中未查到用户。
                console.log(error);
              }); 
 
    }

  
   
 



    async created(){
        // //20220420
        // alert('autolCreated'+Cookies.get('last_page_name'));
         console.log('autol'+Cookies.get('last_page_name'));
        setTimeout(() => {
            if (this.validator1()) {
            }
            else
            {
                alert('验证失败！');
                this.$router.push({
                    name: 'login'
         }); 
            }
        }, 10);
 
    }
}
</script>
