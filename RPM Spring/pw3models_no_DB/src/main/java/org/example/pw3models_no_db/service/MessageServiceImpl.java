package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.MessageApiClient;
import org.example.pw3models_no_db.model.MessageModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class MessageServiceImpl implements MessageService {

    private final MessageApiClient messageApiClient;

    public MessageServiceImpl(MessageApiClient messageApiClient) {
        this.messageApiClient = messageApiClient;
    }

    @Override
    public List<MessageModel> findAll() {
        return messageApiClient.findAll();
    }

    @Override
    public MessageModel addMessage(MessageModel message) {
        return messageApiClient.addMessage(message);
    }

    @Override
    public MessageModel updateMessage(MessageModel message) {
        messageApiClient.updateMessage(message);
        return message;
    }

    @Override
    public void deleteMessage(int id) {
        messageApiClient.deleteMessage(id);
    }

    @Override
    public List<MessageModel> findBySenderId(int senderId) {
        return messageApiClient.findBySenderId(senderId);
    }

    @Override
    public MessageModel findById(int id) {
        return messageApiClient.findById(id);
    }
}