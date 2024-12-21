import { FileSortStatus, FileStatus } from "../models/file-models";
import { BaseClient } from "./api";

export class FilesSortingClient extends BaseClient {
  async fileExists(): Promise<boolean> {
    return this.get<boolean>(`/api/file/sorting/file-exists`) as Promise<boolean>;
  }

  async sortFile(): Promise<FileSortStatus> {
    const response = await fetch(`/api/file/sorting/sort`, {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error("Failed to sort file.");
    }

    return response.json();
  }

  async getFileStatus(fileName: string): Promise<FileStatus> {
    return this.get<FileStatus>(
      `/api/file/sorting/file-status?fileName=${encodeURIComponent(fileName)}`
    ) as Promise<FileStatus>;
  }

  async downloadFile(fileName: string): Promise<void> {
    const url = `/api/file/sorting/download?fileName=${encodeURIComponent(
      fileName
    )}`;
    window.open(url, "_blank");
  }

  async deleteFiles(): Promise<void> {
    return this.delete<void>(`/api/file/sorting/delete`);
  }
}
