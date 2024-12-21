export interface FileCreatedStatus {
  status: number;
  fileName: string;
  sortTimeInSeconds: number;
}

export interface FileStatus {
  status: number;
  sortTimeInSeconds: number;
}

export interface FileUploadStatus {
  fileName: string;
  uploaded: boolean;
}

export interface FileSortStatus {
  fileName: string;
  status: number;
  sortTimeInSeconds: number;
}