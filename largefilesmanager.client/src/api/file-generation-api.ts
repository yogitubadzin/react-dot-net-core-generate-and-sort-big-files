import {
  FileCreatedStatus,
  FileStatus,
} from "../models/file-models";
import { BaseClient } from "./api";

export class FileGenerationClient extends BaseClient {
  async generateFile(fileSize: number): Promise<FileCreatedStatus> {
    return this.post<FileCreatedStatus>("/api/file/generation/generate", {
      fileSize,
    }) as Promise<FileCreatedStatus>;
  }

  async getFileStatus(fileName: string): Promise<FileStatus> {
    return this.get<FileStatus>(
      `/api/file/generation/file-status?fileName=${encodeURIComponent(fileName)}`
    ) as Promise<FileStatus>;
  }

  async downloadFile(fileName: string): Promise<void> {
    const url = `/api/file/generation/download?fileName=${encodeURIComponent(
      fileName
    )}`;
    window.open(url, "_blank");
  }

  async deleteFiles(): Promise<void> {
    return this.delete<void>(`/api/file/generation/delete`);
  }
}
