import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Archiveboxlist from '../entities/archiveboxlist'
import Recordlist from '../entities/recordlist'
import User from '../entities/user'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'
class PickDto {      
    storageId: number;
    goodsId: number;    
    userid: number;     
    constructor ( sid: number, gid: number ) {
        this.storageId =sid;
        this.goodsId =gid;
    };

}
interface ArchiveboxlistState extends ListState<Archiveboxlist>{
    editArchiveBoxlist:Archiveboxlist;
    bindingArchiveBoxlist:Archiveboxlist;
    users:User[];
    plist:PickDto[];
    recordlists:Recordlist[]
}
class ArchiveboxlistModule extends ListModule<ArchiveboxlistState,any,Archiveboxlist>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Archiveboxlist>(),
        users: new Array<User>(),
        loading:false,
        editArchiveBoxlist:new Archiveboxlist(),
        bindingArchiveBoxlist:new Archiveboxlist(),
        plist:new Array<PickDto>(),
        recordlists:new Array<Recordlist>()
    }
    actions={
        async getAll(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/StorageLists/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Archiveboxlist>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            await Ajax.post('/api/services/app/StorageLists/Create',payload.data);
        },
        async update(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            await Ajax.put('/api/services/app/StorageLists/Update',payload.data);
        },
        async delete(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            await Ajax.delete('/api/services/app/StorageLists/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/StorageLists/Get?Id='+payload.id);
            return reponse.data.result as Archiveboxlist;
        },
        async pick(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/ManageMains/PickOutTask',payload.data);
        }, 
        async getUsers(context:ActionContext<ArchiveboxlistState,any>){
            let reponse=await Ajax.get('/api/services/app/User/GetAll');
            context.state.users=reponse.data.result.items as User[];
        },
        async getrecordLists(context:ActionContext<ArchiveboxlistState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/Archives/GetRecordListsByGoodsId?mainId='+payload.id);
            context.state.recordlists=reponse.data.result.items as Recordlist[];
        }

        
    };
    mutations={
        setCurrentPage(state:ArchiveboxlistState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:ArchiveboxlistState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:ArchiveboxlistState,archiveboxlist:Archiveboxlist){
            state.editArchiveBoxlist=archiveboxlist;
        },
        bind(state:ArchiveboxlistState,archiveboxlist:Archiveboxlist){
            state.bindingArchiveBoxlist=archiveboxlist;
        },
        plistadd(state:ArchiveboxlistState,plistadd:Array<PickDto>){
            state.plist=plistadd;
            console.log(state.plist);
        }
    }
}
const archiveboxlistModule=new ArchiveboxlistModule();
export default archiveboxlistModule;