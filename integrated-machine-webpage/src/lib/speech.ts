const msg = new SpeechSynthesisUtterance();


// 语音播报的函数
// text – 要合成的文字内容，字符串。
// lang – 使用的语言，字符串， 例如："zh-cn"
// voiceURI – 指定希望使用的声音和服务，字符串。
// volume – 声音的音量，区间范围是0到1，默认是1。
// rate – 语速，数值，默认值是1，范围是0.1到10，表示语速的倍数，例如2表示正常语速的两倍。
// pitch – 表示说话的音高，数值，范围从0（最小）到2（最大）。默认值为1。
export function handleSpeak(text, rate) {
  msg.text = text; // 语音播报文字内容
  msg.lang = "zh-CN"; // 使用的语言:中文
  msg.volume = 1; // 声音音量：1
  msg.rate = 1; // 语速：1
  msg.pitch = 1; // 音高：1
  window.speechSynthesis.speak(msg); // 播放
}


// 语音停止
export function handleStop() {
  //msg.text = e;
  msg.lang = "zh-CN";
  window.speechSynthesis.cancel();
}
