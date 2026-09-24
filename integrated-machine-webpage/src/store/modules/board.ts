import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Board from '../entities/board'
import Stocktask from '../entities/stocktask'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'
class StocksDto {      
    boxTotalCt: number;
    boxInTotalCt: number;    
    archiveTotalCt: number;    
    archiveInTotalCt: number;     
    borrowedTotalCt: number;      
}
class CellsDto {      
    fullTotalCt: number;
    emptyTotalCt: number;       
}
class RecordsDto {      
    totalCount: number=0;
    keys: string[]=new Array<string>();   
    value: number[]=new Array<number>();    
    invalue: number[]=new Array<number>();    
    outvalue: number[]=new Array<number>();        
}
class DeviceStatusDto {      
    temp: string;
    hum: string;
    status: string;
    columnStatus:string; 
    columnAnimation:string; 
    robotStatus:string;            
}
interface BoardState extends ListState<Board>{
    stocks:StocksDto[];
    cells:CellsDto[];
    records:RecordsDto[];
    deviceStatusDto:DeviceStatusDto[];
    doorstocktasks:Stocktask[];
    robotstocktask:Stocktask
}
class BoardModule extends ListModule<BoardState,any,Board>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Board>(),
        loading:false,
        stocks:new StocksDto(),
        cells:new CellsDto(),
        records:new RecordsDto(),
        deviceStatusDto:new DeviceStatusDto(),
        doorstocktasks:new Array<Stocktask>(),
        robotstocktask:new Stocktask()
    }
    actions={
        async getAll(context:ActionContext<BoardState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/StorageLists/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Board>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<BoardState,any>,payload:any){
            await Ajax.post('/api/services/app/StorageLists/Create',payload.data);
        },
        async update(context:ActionContext<BoardState,any>,payload:any){
            await Ajax.put('/api/services/app/StorageLists/Update',payload.data);
        },
        async delete(context:ActionContext<BoardState,any>,payload:any){
            await Ajax.delete('/api/services/app/StorageLists/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<BoardState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/StorageLists/Get?Id='+payload.id);
            return reponse.data.result as Board;
        },
        async pick(context:ActionContext<BoardState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/ManageMains/PickOutTask',payload.data);
        }, 
        async getStocks(context:ActionContext<BoardState,any>){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetStocksCt');
            //console.log(reponse);
            context.state.stocks=reponse.data.result as StocksDto[];
                        console.log('getStocks');
        }, 
        async getCells(context:ActionContext<BoardState,any>){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetCellsCt');
            //console.log(reponse);
            context.state.cells=reponse.data.result as CellsDto[];
            console.log('getCells');
        }, 
        async getRecords(context:ActionContext<BoardState,any>){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetRecordsCt');
            //console.log(reponse);
            context.state.records=reponse.data.result as RecordsDto[];
            console.log('getRecords');
        }, 
        async getCabinetStatus(context:ActionContext<BoardState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetCabinetStatus?cabinetQuNo='+payload.no);
            //console.log(reponse);
            context.state.deviceStatusDto=reponse.data.result as DeviceStatusDto[];
            console.log('CabinetStatus');
        },
        async getAllInOutTask(context:ActionContext<BoardState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetAllInOutTask');
            context.state.doorstocktasks=reponse.data.result.items as Stocktask[];
        },
        async getCurrentRobotTask(context:ActionContext<BoardState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetCurrentRobotTask');
            context.state.robotstocktask=reponse.data.result as Stocktask;
        }

        
    };
    mutations={
        setCurrentPage(state:BoardState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:BoardState,pagesize:number){
            state.pageSize=pagesize;
        }
    }
}
const boardModule=new BoardModule();
export default boardModule;