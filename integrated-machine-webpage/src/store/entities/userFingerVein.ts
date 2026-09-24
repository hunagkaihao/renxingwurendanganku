import Entity from './entity'
export default class UserFingerVein extends Entity<number>{
    userId:number;
    veinNo:number;  
    featureData:string;  
}