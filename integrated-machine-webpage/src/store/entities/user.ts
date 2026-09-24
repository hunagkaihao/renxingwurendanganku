import Entity from './entity'
export default class User extends Entity<number>{
    password:string;
    facePath:string;
    emailAddress:string;
    surname:string;
    userName:string;
}