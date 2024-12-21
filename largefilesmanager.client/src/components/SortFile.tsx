import React, { memo, useEffect, useState } from "react";
import { FilesSortingClient } from "../api";
import useElapsedTimeFormatter from "../hooks/time-formatter-hook";
import { FileCreatedStatus, FileStatus } from "../models";

const SortFile: React.FC = () => {
  const [uploaded, setUploaded] = useState(false);
  const [sorting, setSorting] = useState(false);
  const [status, setStatus] = useState(0);
  const [fileName, setFileName] = useState("");
  const [elapsedTime, setElapsedTime] = useState<number | null>(null);
  const [sortTime, setSortTime] = useState<number | null>(null);

  const newFileCreatedTime = useElapsedTimeFormatter(elapsedTime || 0);
  const sortFileTime = useElapsedTimeFormatter(sortTime || 0);
  const filesClient = new FilesSortingClient();

  useEffect(() => {
    const checkIfFileExists = async () => {
      try {
        const exists = await filesClient.fileExists();
        setUploaded(exists);
      } catch (error) {
        console.error("Error checking if file exists:", error);
      }
    };

    checkIfFileExists();
  }, []);

  const checkFileStatus = async (fileNameParam: string, startTime: number) => {
    try {
      const data: FileStatus = await filesClient.getFileStatus(fileNameParam);
      setStatus(data.status);

      if (data.status === 100) {
        setSortTime(data.sortTimeInSeconds * 1000);
        setSorting(false);
        const endTime = Date.now();
        if (startTime) {
          const elapsed = endTime - startTime;
          setElapsedTime(elapsed);
        }
        return;
      }

      setTimeout(() => checkFileStatus(fileNameParam, startTime), 1000);
    } catch (error) {
      console.error("Error fetching file status:", error);
      setSorting(false);
    }
  };

  const handleSort = async () => {
    if (sorting || !uploaded) return;

    try {
      const now = Date.now();
      setSorting(true);
      setStatus(0);
      setElapsedTime(null);

      const data: FileCreatedStatus = await filesClient.sortFile();

      setFileName(data.fileName);
      checkFileStatus(data.fileName, now);
    } catch (error) {
      console.error("Error starting file sorting:", error);
      setSorting(false);
    }
  };

  const triggerDownload = async () => {
    try {
      await filesClient.downloadFile(fileName);
    } catch (error) {
      console.error("Error downloading the file:", error);
    }
  };

  const clearStates = async () => {
    setStatus(0);
    setSorting(false);
    setFileName("");
    setElapsedTime(null);
    setUploaded(false);

    await filesClient.deleteFiles();
  };

  return (
    <div>
      <h2>Sort File</h2>
      <div>
        <button onClick={handleSort} disabled={!uploaded || sorting}>
          Sort File
        </button>
      </div>

      {sorting && (
        <div>
          <p>Sorting file... Progress: {status}%</p>
          <progress
            value={status}
            max="100"
            style={{ width: "100%" }}
          ></progress>
        </div>
      )}

      {status === 100 && (
        <div>
          <p>File {fileName} was sorted successfully!</p>
          <p>File was recreated to new file and sorted in time: {newFileCreatedTime}</p>
          <p>File was sorted in time: {sortFileTime}</p>
          <button onClick={triggerDownload}>Download File to view</button>
          <br />
          <button onClick={clearStates}>Clear</button>
        </div>
      )}
    </div>
  );
};

const _SortFile = memo(SortFile);

export { _SortFile as SortFile };
