<template>
    <div v-show="isshow" dis-hover style="height:800px;" id='building'>
            <div class="page-body2" >
        <div class="margin-top-1">
             <div class="main-header2">
                    <div class="header-middle-con" style="margin-left: -25px;margin-top: 1px;"><h1>人脸验证</h1> </div>
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
          
            <div style="margin:140px;">
                              <!-- <div
                              v-show="canvasShow"
                              id="videomask"
                              class="videomask"
                            ></div> -->
                            <canvas
                            v-show="false"
                              id="canvas"
                              width="336px"
                              height="189px"
                            ></canvas>
                            <video
                              
                              id="video"
                              width="336px"
                              height="189px"
                              class="unlock-avator-img"
                            ></video>
                <!-- <img id="video" src="../../images/face.png" > -->
                <h2 style="text-align: center; margin:20px">{{mge}}</h2>
                
                <div style="margin-left: 128px;margin-top: 20px;">
                    <img src="../../images/face.png" width="80" @click="faceTest">
                </div>
                <!-- <Icon @click="logout" type="md-return-left" size="150"  color="LightSkyBlue" /> -->
            </div>
          
        </Row>
    </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import AbpBase from '../../lib/abpbase'
import Util from '../../lib/util';
import Cookies from 'js-cookie';
import axios from 'axios'
import {handleSpeak,handleStop} from '../../lib/speech'
@Component
export default class UnLock extends AbpBase {
    name:string='Unlock';
    avatorLeft:string='0px';
    inputLeft:string= '400px';
    password:string= '';
    check=null;
    isshow:boolean = false
    mge:String='人脸验证中'
    msg = new SpeechSynthesisUtterance();

//  handleSpeak(text, rate) {
//   this.msg.text = text; // 语音播报文字内容
//   this.msg.lang = "zh-CN"; // 使用的语言:中文
//   this.msg.volume = 1; // 声音音量：1
//   this.msg.rate = 1; // 语速：1
//   this.msg.pitch = 1; // 音高：1
//   console.log("speak调用")
//   window.speechSynthesis.speak(this.msg); // 播放
//   console.log("speak调用")
// }

    @Prop({type:Boolean,default:false}) showUnlock:boolean;

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

    getMedia2() {
      
    this.imgData = "";
    let video = document.getElementById("video") as any;
    let constraints = {
      //AI摄像头必须是这个分辨率，其它会不显示
      video: { width: 336, height: 189 },
      audio: false,
    };
    let promise = navigator.mediaDevices.getUserMedia(constraints);
    promise.then(function (MediaStream: any) {
        MediaStreamTrack =
          typeof MediaStream.stop === "function"
            ? MediaStream
            : MediaStream.getTracks()[0];
        video.srcObject = MediaStream;
        video.play();
      })
      .catch(function (PermissionDeniedError) {
        //console.log(PermissionDeniedError);
      });
      
  }
  
	closeMedia() {
        MediaStreamTrack && (MediaStreamTrack as any).stop();
               // this.mge='验证结束';
	}
    get avatorPath(){
        return localStorage.avatorImgPath;
    }
     //获取验证身份方式
    get ClientVerifyMethod(){
        return abp.setting.values.ClientVerifyMethod
    }
    imagedata:string;

 
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
    faceTestFlag: boolean = false;
  canvasShow: boolean = false;
  userId: number = 0;
  imgData: string;
  video: any;
  timerCallback() {
    //  let video = document.getElementById("video") as any;
    if (this.video.paused || this.video.ended) {
      return;
    }
    if (this.faceTestFlag) {
      return;
    }
    let canvas = document.getElementById("canvas") as any;
    let ctx = canvas.getContext("2d");
    ctx.drawImage(this.video, 0, 0, 336, 189);
    var type = "image/jpeg";
    //从画布上获取照片数据
    this.imgData = canvas.toDataURL(type);
    //   this.computeFrame();
    let self = this;
    setTimeout(function () {
      self.timerCallback();
    }, 0);
  }
  async faceTest() {
    this.canvasShow = true;
    this.faceTestFlag = false;
    this.mge = "人脸验证开始";
    this.getMedia2();
    //handleSpeak("人脸识别开始",1);
    let times = 15;
    //console.time("runTime:");
    for (let index = 0; index < times; index++) {
      if (index > 0) {
        this.canvasShow = false;
      }
      await this.sleep(350).then(async () => {
        // 这里写sleep之后需要去做的事情

        //console.log(index.toString() + new Date());
        //console.time("test" + index);
      });
      //console.timeEnd("test" + index);
      //console.log(index.toString() + new Date());
      //   console.log("ss"+ await this.validatorphoto(index).then());
      if (index > 0) {
        await this.validatorphoto(index)
          .then(() => {
            this.faceTestFlag = true;
          })
          .catch(function (error) {
            //alert(error );
            //console.log(error);
            //return false;
          });
      }
      if (this.faceTestFlag) {
        index = 9999;
        //console.timeEnd("runTime:");
        //  this.canvasShow=true;
        //  this.closeMedia();

        return;
      }
    }
    //console.timeEnd("runTime:");
    this.mge = "人脸验证超时,点击人脸图标继续";
    //handleStop();
    //handleSpeak("人脸验证超时,点击人脸图标继续",1);
    // var videomask = document.getElementById('videomask');
    // videomask.style.backgroundImage ="url('')";
    // this.canvasShow=true;
    this.closeMedia();
  }
  async validatorphoto(index: number): Promise<any> {
    //console.time("validatorphoto" + index);
    let rep = await axios
      .post("http://localhost:5032/Face/FaceMatch", {
        imageData: this.imgData.substr(23),
      })
      .then(async (response) => {

        //人脸识别成功
        if (response.data > 0) {
            this.mge = "人脸测试成功" + response.data;
            Cookies.set('facing', response.data);
            this.faceTestFlag = true;
            //this.canvasShow=true;
            if(this.ClientVerifyMethod == "FaceAndVein" ){
              setTimeout(() => {
                  this.$router.push({
                      name: 'finger'
                  });
                  this.closeMedia();
              }, 1000);
            }else{
              await this.$store.dispatch({
                     type:'app/aifaceauto',
                      data:{
                          Userid:Cookies.get('facing'),  
                          rememberMe:false
                           }
                    }) 

                     await this.$store.dispatch({
                            type:'session/init'
                        })
                Cookies.set('userId', Cookies.get('facing'))
                this.$router.push({
                  name: Cookies.get('last_page_name')
              });
            }
            return Promise.resolve("人脸测试成功");
            

        } else {
          //("人脸识别失败");
          return Promise.reject("人脸识别失败");
          //return false;
        }
        // console.log(response.data.ret_code);
        //console.log(response);
      })
      .catch(function (error) {
        //console.timeEnd("validatorphoto" + index);
        //console.log(error);
        return Promise.reject("人脸识别失败");
        //return false;
      });
  }
  async sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
  }

    async created(){
      console.debug("created");

        
    }
    async mounted() {
      console.debug("mounted");
    this.video = document.getElementById("video") as any;
    this.video.addEventListener("play", this.timerCallback, false);
    this.faceTest();
    
    setTimeout(()=>{
      this.isshow = true;
    },1300)
  }
    async destroyed() {
    this.closeMedia();
  }
}
</script>
<style lang="less" scoped>
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