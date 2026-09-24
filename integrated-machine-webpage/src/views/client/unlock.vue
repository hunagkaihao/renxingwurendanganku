<style lang="less">
    @import './styles/unlock.less';
</style>
<template>
    <transition name="show-unlock">
        <div class="unlock-body-con" v-if="showUnlock" @keydown.enter="handleUnlock">
            <div @click="handleClickAvator" class="unlock-avator-con" :style="{marginLeft: avatorLeft}">
                <video id="video" class="unlock-avator-img"  autoplay="autoplay">您的浏览器不支持 video 标签</video>
                <div  class="unlock-avator-cover">
                    <span><Icon type="unlocked" :size="30"></Icon></span>
                    <p>{{L('UnLock')}}</p>
                </div>
            </div>
            <div class="unlock-avator-under-back" :style="{marginLeft: avatorLeft}"></div>
                       <canvas hidden="hidden" id="canvas" width="300px" height="300px"></canvas>
            <div class="unlock-input-con">

            </div>
            <div class="unlock-locking-tip-con">{{ mge }}</div>
        </div>
    </transition>
</template>
<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import AbpBase from '../../lib/abpbase'
import Util from '../../lib/util';
import appconst from '../../lib/appconst'
import Cookies from 'js-cookie';
import axios from 'axios'
@Component
export default class UnLock extends AbpBase {
    name:string='Unlock';
    avatorLeft:string='0px';
    inputLeft:string= '400px';
    password:string= '';
    check=null;
 
    mge:String='人脸验证中'
    @Prop({type:Boolean,default:false}) showUnlock:boolean;


        getMedia2() {
        let video = document.getElementById("video") as any;
        let constraints = {
            video: {width: 500, height: 500},
            audio: false
        };
        let promise = navigator.mediaDevices.getUserMedia(constraints);
        promise.then(function (MediaStream:any) {
			  MediaStreamTrack = typeof MediaStream.stop === 'function'?MediaStream:MediaStream.getTracks()[0];
            video.srcObject = MediaStream;
            video.play();
        }).catch(function (PermissionDeniedError) {
            console.log(PermissionDeniedError);
        })
    }
        takePhoto() {
        //获得Canvas对象
        let canvas = document.getElementById("canvas") as any;
        let video = document.getElementById("video") as any;
        let ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0, 300, 300);
        var type='image/jpeg';
        		//从画布上获取照片数据  
		var imgData = canvas.toDataURL(type);  
		//将图片转换为Base64  
		this.imagedata = imgData.substr(23);
		//console.log("base64Data:"+imgData);
    }
     takePhoto1() {
        //获得Canvas对象
        let canvas = document.getElementById("canvas") as any;
        let video = document.getElementById("video") as any;
        let ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0, 300, 300);
        var type='image/jpeg';
        		//从画布上获取照片数据  
		var imgData = canvas.toDataURL(type);  

        //console.log("22base64Data:"+imgData);
        		//将图片转换为Base64  
		return imgData.substr(23);
    }
	closeMedia() {
        MediaStreamTrack && (MediaStreamTrack as any).stop();
               // this.mge='验证结束';
	}
    get avatorPath(){
        return localStorage.avatorImgPath;
    }
    imagedata:string;
    async  validator () {
        let loginModel={
            image:this.imagedata,
            rememberMe:false
        }
        await this.$store.dispatch({
            type:'app/face',
            data:loginModel
        }) 
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

            console.log(Cookies.get('last_page_name'));
            console.log('validator');

            return true;
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
                    this.closeMedia();
                        this.mge= this.$store.state.session.user?this.$store.state.session.user.name:'';
                        this.$emit('on-unlock');
                        return true;
              })
              .catch( (error) => {
                    this.validator2()
                console.log(error);
              }); 
 
    }

        async  validator2 () {
              //this.$Message.success('任务下达成功。');
            console.log('validator2');
            let rep =  axios.post('/RecognitionManage', {
                "RequestValue":  2819,
                "pass":  "123456",
                "image_type": "image",
                "quality_control": "NONE",
                "image_content":this.imagedata
                })
              .then( async (response) => {
                  //人脸识别成功
                  if(response.data.ret_code==0)
                  {

                    if(response.data.user_list[0].score>0.85)
                    {                  
                     await   this.$store.dispatch({
                                    type:'app/aifaceauto',
                                    data:{
                                        Userid:response.data.user_list[0].user_id,  
                                        rememberMe:false
                                    }
                                }) 

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

                            console.log(Cookies.get('last_page_name'));
                            console.log('validator2');
                             this.closeMedia();
                              this.mge= this.$store.state.session.user?this.$store.state.session.user.name:'';
                            //alert(this.$store.state.session.user.name);
                            this.$emit('on-unlock');
                            return true;
                    }
                    else
                    {
                        //人脸识别失败
                        //alert("人脸识别失败");
                        this.$Message.info("人脸识别失败");
                    }
                  }
                  else
                  {
                            let rep =  axios.post('/RecognitionManage', {
                            "RequestValue":  2819,
                            "pass":  "123456",
                            "image_type": "image",
                            "quality_control": "NONE",
                            "image_content":this.takePhoto1()
                            })
                        .then( async (response) => {
                            //人脸识别成功
                            if(response.data.ret_code==0)
                            {
                                if(response.data.user_list[0].score>0.85)
                                {  
                                
                                await   this.$store.dispatch({
                                                type:'app/aifaceauto',
                                                data:{
                                                    Userid:response.data.user_list[0].user_id,  
                                                    rememberMe:false
                                                }
                                            }) 

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

                                        console.log(Cookies.get('last_page_name'));
                                        console.log('validator2');
                                        this.closeMedia();
                                        this.mge= this.$store.state.session.user?this.$store.state.session.user.name:'';
                                        //alert(this.$store.state.session.user.name);
                                        this.$emit('on-unlock');
                                        return true;
                                }
                                 else
                                {
                                    //人脸识别失败
                                    this.$Message.info("人脸识别失败");
                                    //alert("人脸识别失败");
                                }
                            }
                            else
                            {
                                                      //人脸识别失败
                                    this.$Message.info("人脸识别失败");
                                             //alert("人脸识别失败");
                            }

                        })
                        .catch(function (error) {
                            console.log(error);
                        });
                    //   //人脸识别失败

                    //   alert("人脸识别失败");
                  }
                                // console.log(response.data.ret_code); 
                console.log(response);
              })
              .catch(function (error) {
                console.log(error);
              });
                     

    }
    handleClickAvator () {
        this.avatorLeft = '-180px';
        this.inputLeft = '0px';
        this.mge='验证结束';
               // this.getMedia2();
                        alert(2);
    }
    async handleUnlock () {
        alert(0);
    }
    unlockMousedown () {
        (this.$refs.unlockBtn as any).className = 'unlock-btn click-unlock-btn';
    }
    unlockMouseup () {
        (this.$refs.unlockBtn as any).className = 'unlock-btn';
    }



    async created(){
        setTimeout(() => {
            abp.utils.setCookieValue('Abp.Localization.CultureName','zh-Hans',new Date(new Date().getTime() + 5 * 365 * 86400000),Util.abp.appPath);
            this.getMedia2();
        }, 1200);
        setTimeout(() => {
            this.takePhoto();
        }, 3600);
        setTimeout(() => {
            if (this.validator1()) {
                //this.mge='验证成功';

            //alert(2);
            //location.reload();
            }
            else
            {
                alert('验证失败！');
                this.$router.push({
                    name: 'login'
         }); 
            }
        }, 5600);
        // setTimeout(() => {
        //     if (this.validator()) {
        //         //this.mge='验证成功';

        //     //alert(2);
        //     //location.reload();
        //     }
        //     else
        //     {

        //         this.$router.push({
        //             name: 'login'
        //  }); 
        //     }
        // }, 5600);
        setTimeout(() => {
            this.closeMedia();
        }, 10600);
        setTimeout(() => {
            this.mge= this.$store.state.session.user?this.$store.state.session.user.name:'';
            //alert(this.$store.state.session.user.name);
            this.$emit('on-unlock');
        }, 12600);
    }
}
</script>
