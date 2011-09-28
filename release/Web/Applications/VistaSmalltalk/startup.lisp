(sys::in-package "SYSTEM")

(setq ext:*autoload-verbose* nil)
(setq *load-verbose* nil)

(defmacro lambda (lambda-list &rest body)
  (list 'function (list* 'lambda lambda-list body)))

(defmacro when (pred &rest body)
  (list 'if pred (if (> (length body) 1)
                     (append '(progn) body)
                     (car body))))

(defmacro unless (pred &rest body)
  (list 'if (list 'not pred) (if (> (length body) 1)
                                 (append '(progn) body)
                                 (car body))))

(defmacro return (&optional result)
  (list 'return-from nil result))

(defmacro defun (name lambda-list &rest body)
  (list 'sys::%defun (list 'QUOTE name) (list 'QUOTE lambda-list) (list 'QUOTE body)))

(defmacro defconstant (name initial-value &optional docstring)
  (list 'sys::%defconstant (list 'QUOTE name) initial-value docstring))

(defmacro defparameter (name initial-value &optional docstring)
  (list 'sys::%defparameter (list 'QUOTE name) initial-value docstring))

(defun print (object &optional stream)
  (terpri stream)
  (let ((*print-escape* t))
    (output-object object stream))
  (write-char #\space stream)
  object)

(defun prin1 (object &optional (stream *standard-output*))
  (let ((*print-escape* t))
    (output-object object stream))
  object)

(defun princ (object &optional (stream *standard-output*))
  (let ((*print-escape* nil)
        (*print-readably* nil))
    (output-object object stream))
  object)

(defun simple-format (destination control-string &rest args)
  (apply *simple-format-function* destination control-string args))

(export 'simple-format '#:system)
