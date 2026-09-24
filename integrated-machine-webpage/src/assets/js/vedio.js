function tableToExcel() {
   alert("aa");
  
}


function getMedia() {
    var video = document.querySelector('video')
     var canvas2 = document.getElementById('canvas');
    var context2 = canvas2.getContext('2d');
            // vedio播放时触发，绘制vedio帧图像到canvas
    video.addEventListener('play', function () {
            window.setInterval(function () {
                context2.drawImage(video, 0, 0, 370, 370);
            }, 20);
         }, false);
    navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia;
window.URL = window.URL || window.webkitURL || window.mozURL || window.msURL;
    if (navigator.getUserMedia) {
        navigator.getUserMedia({
                audio: false,
                video:  {
                    'optional': [{
                        'sourceId': ""//exArray[1] //0为前置摄像头，1为后置
                    }]
                }
            },
            function(stream) {
                alert(video);
                //mediaStreamTrack = typeof stream.stop === 'function' ? stream : stream.getTracks()[1];
                //alert('Succeed to get media!');
                if (video.mozSrcObject !== undefined) {
                    //Firefox中，video.mozSrcObject最初为null，而不是未定义的，我们可以靠这个来检测Firefox的支持
                    video.mozSrcObject = stream;
                }
                else {
                   
                    try {
                        video.src = window.URL.createObjectURL(stream);
                    } catch (e) {
                        video.srcObject = stream;
                    }
                   // video.src = window.URL && window.URL.createObjectURL(stream) || stream;
                }
            },
            function(err) {
                alert('Native device media streaming (getUserMedia) not supported in this browser.');
                console.log(err)
            }
        )
    }
    else {
        alert('Native device media streaming (getUserMedia) not supported in this browser.');
    }
}
function takePhoto() {
    //获得Canvas对象
    let canvas = document.getElementById("canvas");
    let ctx = canvas.getContext('2d');
    ctx.drawImage(video, 0, 0, 370, 370);
    var imgData = canvas.toDataURL();  
    //将图片转换为Base64  
    var base64Data = imgData.substr(22);
    console.log("base64Data:"+base64Data);
}
function closeMedia() {
    this.MediaStreamTrack && this.MediaStreamTrack.stop();
}
function toBase64() {
    //获得Canvas对象
    let canvas = document.getElementById("canvas");
    //从画布上获取照片数据  
    var imgData = canvas.toDataURL();  
    //将图片转换为Base64  
    var base64Data = imgData.substr(22);
    console.log("base64Data:"+base64Data);
}

export {
    tableToExcel,
    getMedia,
    takePhoto,
    closeMedia,
    toBase64
}