package org.example.pw3models_no_db.service;
import org.example.pw3models_no_db.model.MessageModel;

import java.util.List;

public interface MessageService {
    List<MessageModel> findAll();
    MessageModel addMessage(MessageModel message);
    MessageModel updateMessage(MessageModel message);
    void deleteMessage(int id);
    List<MessageModel> findBySenderId(int senderId);
    MessageModel findById(int id);
}