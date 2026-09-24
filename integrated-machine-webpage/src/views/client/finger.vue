<template>
    <div dis-hover style="height:800px" id='building'>
            <div class="page-body2">
        <div class="margin-top-1">
             <div class="main-header2">
                    <div class="header-middle-con" style="margin-left: -25px;margin-top: 1px;"><h1>指静脉验证</h1> </div>
                        <div class="header-avator-con" style="margin-right: 10px;">     

                            <div class="user-dropdown-menu-con">
                                <Row type="flex" justify="end" align="middle" class="user-dropdown-innercon" style="margin-left: -10px;">
                                
                                    <Icon @click="logout" type="md-return-left" size="28" style="margin-left: 10px;margin-top: 5px;" color="LightSkyBlue" />

                                </Row>
                            </div>

                        </div>
               </div>
        </div>
        <Row  type="flex" justify="center">
            <div style="margin:200px; text-align: center;">
            <!-- <div style="margin:20px; text-align: center;"> -->
                <!-- <img src="../../images/finger.png" > -->
                
                
                
                    <img v-show="png" src="../../images/指纹.png" width="80" @click="reset">
                    <img v-show="!png" src="../../images/指纹2.png" width="80" @click="reset">
                    <h2 style="margin:20px; text-align: center;">{{msg}}</h2>
                <!-- <Icon @click="logout" type="md-return-left" size="150"  color="LightSkyBlue" /> -->
            </div>
            
        </Row>
    </div>

    </div>
</template>

<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import Cookies from 'js-cookie';
import Util from '@/lib/util';
import AbpBase from '../../lib/abpbase'
import axios from "axios";
@Component
export default class finger extends AbpBase  {

    msg:string = "指静脉验证中";
    png:boolean = true;
        
    ab = axios.CancelToken.source()
       get list2() { 
            return this.$store.state.user.userFingerVeins;
        }
  
        logout()
        {   
            this.$store.commit('app/logout', this);
            Util.abp.auth.clearToken();
            Cookies.set('facing', '0');
            Cookies.set('userId', '0');
            // location.reload();
            this.$router.push({
                name: 'login'
            });
           // location.reload();
        }
    testcount = 0;
    veintest() {
    axios.get('http://localhost:5032/Vein/VeinMatch',{cancelToken:this.ab.token})
        .then(async (response) => {
          // this.serviceConnect=false;
          //console.log(response);
          //console.log(this.list2[0].id)
          let a = this.list2.find((f) =>
            f.id == response.data
          )
          console.log(response.data)
        //   if(response.data == 0){
        //     this.msg = "指静脉反复，点击图标继续"
        //     return;
        //   }
          //指纹用户和人脸用户一致时跳转
          if (response.data == -1 ) {
            if(this.testcount < 4){
                this.testcount ++;
                this.msg = "验证中"
                setTimeout(() => {
                    this.veintest()
                }, 1000);
            }else{
                this.msg = "指静脉验证失败，点击图标继续"
                this.testcount = 0
            }
            
          }
          else if (response.data != 0 && a != undefined ) {
                await   this.$store.dispatch({
                    type:'app/aifaceauto',
                      data:{
                          Userid:Cookies.get('facing'),  
                          rememberMe:false
                        }
                    }) 

                     await this.$store.dispatch({
                            type:'session/init'
                        })
            this.testcount = 0
            Cookies.set('userId', Cookies.get('facing'))
            this.$router.push({
                name: Cookies.get('last_page_name')
            });
          } 
          else {
            if(this.testcount < 4){
                this.testcount ++;
                this.msg = "验证中"
                setTimeout(() => {
                    this.veintest()
                }, 1000);
            }else{
                this.msg = "指静脉与人脸不匹配，点击图标继续"
                this.testcount = 0
            }
          }
        })
        .catch((error) => {

            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }

        });
  }

    reset(){
        this.msg = "指静脉验证中"
        this.veintest()
        }
	
    handleUnlock () {
            Cookies.set('facing', '0');

            this.$router.push({
                name: Cookies.get('last_page_name')
            });
      }
    
   async mounted () {
       await this.$store.dispatch({
            type: "user/getVeins",
            userId: Cookies.get("facing"),
        });

        setInterval(()=>{
            this.png =!this.png
        },1000)
        

        this.veintest();
    }

    beforeDestroy() {
        this.ab.cancel('cancel request before enter next page')
    }

};
</script>
<style scoped>
#building{
  background:url("../../images/background.jpg");
  width:100%;
  height:100%;
  position:fixed;
  background-size:100% 100%;
}
h1{
  color: #fff;
}
</style>
