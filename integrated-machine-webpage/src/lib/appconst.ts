import url from './url'
const AppConsts= {
    /** 仅接入 WMS；开发环境默认启用，显式设为 false 才启用旧设备联调。 */
    wmsOnly: process.env.VUE_APP_WMS_ONLY === 'true' ||
        (process.env.NODE_ENV === 'development' && process.env.VUE_APP_WMS_ONLY !== 'false'),
    userManagement:{
        defaultAdminUserName: 'admin'
    },
    localization:{
        defaultLocalizationSourceName: 'DAManage'
    },
    authorization:{
        encrptedAuthTokenName: 'enc_auth_token'
    },
    appBaseUrl: "http://localhost:8080",
    remoteServiceBaseUrl: url.endsWith('/') ? url.slice(0, -1) : url
}
export default AppConsts
