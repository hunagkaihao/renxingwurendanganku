const CopyWebpackPlugin = require('copy-webpack-plugin');
const proxy = require('http-proxy-middleware');
module.exports = {
    devServer: {
        open: true,
        port: 8080,
        proxy: {
            '/api': {
                target: 'http://192.168.1.188:21021/api', 
                changeOrigin: true,
                ws: true,
                pathRewrite: {
                    '^/api': ''
                }
            },
            //增加AI摄像头的跨域访问
            //  '/wcs/serviceStatus': {
            //         target: 'http://192.168.1.188:7788/wcs/serviceStatus', 
            //                 changeOrigin: true,
            //                 ws: true,
            //               pathRewrite: {
            //                     '^/wcs/serviceStatus': ''
            //                 } 
            // },
            //增加AI摄像头的跨域访问
            '/RecognitionManage': {
                //target: 'http://192.168.3.99:8080/RecognitionManage', 
                target: 'http://localhost:5016/RecognitionManage', 
                changeOrigin: true,
                ws: true,
              pathRewrite: {
                    '^/RecognitionManage': ''
                }  
            },
            //增加AI摄像头注册人脸的跨域访问
            '/UserManage': {
                            //target: 'http://192.168.3.99:8080/UserManage', 
                            target: 'http://localhost:5016/UserManage',
                            changeOrigin: true,
                            ws: true,
                          pathRewrite: {
                                '^/UserManage': ''
                            }  
                }
        }
        },
    configureWebpack: config => {
      if (process.env.NODE_ENV === 'production') {
        return {
            plugins:[
                new CopyWebpackPlugin([{
                    from:'node_modules/@aspnet/signalr/dist/browser/signalr.min.js',
                    to:'dist'
                },{
                    from:'node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js',
                    to:'dist'
                },{
                    from:'src/lib/abp.js',
                    to:'dist'
                }])
            ]
        }
      } else {
        return {
            plugins:[
                new CopyWebpackPlugin([{
                    from:'node_modules/@aspnet/signalr/dist/browser/signalr.min.js',
                    to:'dist'
                },{
                    from:'node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js',
                    to:'dist'
                },{
                    from:'src/lib/abp.js',
                    to:'dist'
                }])
            ]
        }
      }
    }
}