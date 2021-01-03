import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClipboardService {

  constructor() { }

  public copy(text: string) {
    const fallback = () => {
      var textArea = document.createElement("textarea");
      textArea.value = text;
      textArea.style.top = "0";
      textArea.style.left = "0";
      textArea.style.position = "fixed";
      document.body.appendChild(textArea);
      textArea.focus();
      textArea.select();
      try {
        document.execCommand("copy");
      } catch (err) { }
      document.body.removeChild(textArea);
    };

    if (!navigator.clipboard) {
      fallback();
    } else {
      navigator.clipboard.writeText(text).then(
        () => { },
        (err) => { fallback(); }
      );
    }

  }
}
