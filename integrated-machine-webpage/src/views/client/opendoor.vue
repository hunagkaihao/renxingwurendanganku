<template>
    <div style="width: 100%;height: 100%;">
          <Button type="primary" @click="closeMedia" long size="large" >密码登录</Button>
        <userlogin v-model="createModalShow"></userlogin>
    </div>
</template>
<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import AbpBase from '../../lib/abpbase'
import Util from '../../lib/util';
import appconst from '../../lib/appconst'
import Cookies from 'js-cookie';
import axios from 'axios'
import userlogin from './userlogin.vue'
@Component ({
      components:{userlogin}
    })
export default class opendoor extends AbpBase {
    
            createModalShow:boolean=false;
      //使用AI镜头
       async  backlastpage () {
            let lockScreenBack = document.getElementById('lock_screen_back');
            lockScreenBack.style.zIndex = "-1";
            lockScreenBack.style.boxShadow = '0 0 0 0 #667aa6 inset';
            // this.$router.push({
            //     name: Cookies.get('last_page_name')
            // });
    }

  	closeMedia() {
               this.create();
	};
            create(){
            this.createModalShow=true;
        }
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
                console.log(error);
              }); 
 
    }
 



    async created(){
                 console.log('autol'+Cookies.get('last_page_name'));
        setTimeout(this.backlastpage, 1000);
                // setInterval(this.validator1, 1000);
 
    }
}
</script>
