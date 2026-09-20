import { defHttp } from '/@/utils/http/axios';

/** 物料基础信息。 */
export interface MaterialDto {
  id: number;
  materialCode?: string;
  materialName?: string;
  materialType?: string;
  materialUnit?: string;
  validityDays?: number;
  creatorUserCode?: string;
  materialCreateTime?: string;
}

/** 创建或更新物料时提交的基础信息。 */
export interface CreateMaterialDto extends Omit<MaterialDto, 'id'> {
  id?: number;
}

/** 物料分页查询条件。 */
export interface PagingMaterialListInput {
  filter?: string;
  skipCount?: number;
  pageSize?: number;
  [key: string]: unknown;
}

/** 物料分页查询结果。 */
export interface MaterialPagedResultDto {
  items: MaterialDto[];
  totalCount: number;
}

enum Api {
  Page = '/api/Material/page',
  Create = '/api/Material/create',
  Update = '/api/Material/update',
  Delete = '/api/Material/delete',
}

/** 查询物料基础信息分页列表。 */
export const getMaterialPage = (params: PagingMaterialListInput) =>
  defHttp.post<MaterialPagedResultDto>(
    { url: Api.Page, params },
    { isTransformResponse: false }
  );

/** 新增物料基础信息。 */
export const createMaterial = (params: CreateMaterialDto) =>
  defHttp.post<MaterialDto>({ url: Api.Create, params }, { isTransformResponse: false });

/** 更新物料基础信息。 */
export const updateMaterial = (params: CreateMaterialDto) =>
  defHttp.post<MaterialDto>({ url: Api.Update, params }, { isTransformResponse: false });

/** 删除指定物料。 */
export const deleteMaterial = (id: number) =>
  defHttp.post<void>({ url: Api.Delete, params: { id } }, { isTransformResponse: false });
