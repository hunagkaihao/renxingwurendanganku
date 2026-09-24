import ajax from '../../lib/ajax';
import util from '../../lib/util'
import { adaptApplicationConfiguration } from '../../lib/application-configuration';
import {Store,Module,ActionContext} from 'vuex' 
interface SessionState{
    application:any,
    user:any,
    tenant:any
}
class SessionStore implements Module<SessionState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async init(content:ActionContext<SessionState,any>){
            const rep = await ajax.get('/api/abp/application-configuration');
            const configuration = adaptApplicationConfiguration(rep.data);
            util.abp = util.extend(true, util.abp, configuration.abp);
            // 授权集合整体替换，避免重新加载配置后残留旧权限。
            util.abp.auth.allPermissions = configuration.abp.auth.allPermissions;
            util.abp.auth.grantedPermissions = configuration.abp.auth.grantedPermissions;
            content.state.application = configuration.session.application;
            content.state.user = configuration.session.user;
            content.state.tenant = configuration.session.tenant;
        }
    }
}
const session=new SessionStore();
export default session;
