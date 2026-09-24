import Entity from './entity'
export default class GoodsMain extends Entity<number>{
    goodsCode:string;
    goodsName:string;  
    rfidId:string; 
    goodsConstProperty1:string;
    goodsConstProperty2:string;
    goodsConstProperty3:string;
    goodsConstProperty4:string;
    goodsConstProperty5:string;
    archiveBoxRfid:string;
}