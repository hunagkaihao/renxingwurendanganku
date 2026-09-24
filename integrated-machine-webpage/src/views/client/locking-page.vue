<template>
    <div style="width: 100%;height: 100%;background: #667aa6">
          <Button type="primary" @click="closeMedia" long size="large" >密码登录</Button>
        <div class="unlock-con">
            <unlock :show-unlock="showUnlock" @on-unlock="handleUnlock"></unlock>
        </div>
        <userlogin v-model="createModalShow"></userlogin>
    </div>
</template>

<script>
import unlock from './unlock.vue';
import Cookies from 'js-cookie';
import userlogin from './userlogin.vue'
export default {
    components: {
        unlock,userlogin
    },
    data () {
        return {
            showUnlock: false,
            createModalShow:false
        };
    },
    methods: {
        create(){
            this.createModalShow=true;
        },
                getMedia2() {
        let video = document.getElementById("video");
        let constraints = {
            video: {width: 500, height: 500},
            audio: false
        };
        let promise = navigator.mediaDevices.getUserMedia(constraints);
        promise.then(function (MediaStream) {
			  MediaStreamTrack = typeof MediaStream.stop === 'function'?MediaStream:MediaStream.getTracks()[0];
            video.srcObject = MediaStream;
            video.play();
        }).catch(function (PermissionDeniedError) {
            console.log(PermissionDeniedError);
        })
    },
        takePhoto() {
        //获得Canvas对象
        let canvas = document.getElementById("canvas");
        let ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0, 500, 500);
        		//从画布上获取照片数据  
		var imgData = canvas.toDataURL();  
		//将图片转换为Base64  
		this.imagedata = imgData.substr(22);
		//console.log("base64Data:"+this.imagedata);
    },
	closeMedia() {
               this.create();
                MediaStreamTrack && MediaStreamTrack.stop();
	},
    handleUnlock () {
        //alert('handleUnlock'+Cookies.get('last_page_name'));
            Cookies.set('facing', '0');
            let lockScreenBack = document.getElementById('lock_screen_back');
            this.showUnlock = false;
            lockScreenBack.style.zIndex = -1;
            lockScreenBack.style.boxShadow = '0 0 0 0 #667aa6 inset';
            this.$router.push({
                name: Cookies.get('last_page_name')
            });
      }
    },
    mounted () {
        this.showUnlock = true;
        if (!document.getElementById('lock_screen_back')) {
            let lockdiv = document.createElement('div');
            lockdiv.setAttribute('id', 'lock_screen_back');
            lockdiv.setAttribute('class', 'lock-screen-back');
            document.body.appendChild(lockdiv);
        }
        let lockScreenBack = document.getElementById('lock_screen_back');
        lockScreenBack.style.zIndex = -1;
    }
};
</script>
