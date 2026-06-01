export const VALID_FILE_TYPES = {
  TXT: 'txt',
  DOCX: 'docx',
  PDF: 'pdf'
}

export const getFileType = (file: File): string => 
  file.name.split('.').pop()?.toLowerCase() || ''

export const isFileValid = (file: File): boolean => {
  const fileType = getFileType(file)

  return fileType === VALID_FILE_TYPES.TXT ||
         fileType === VALID_FILE_TYPES.DOCX ||
         fileType === VALID_FILE_TYPES.PDF         
}
