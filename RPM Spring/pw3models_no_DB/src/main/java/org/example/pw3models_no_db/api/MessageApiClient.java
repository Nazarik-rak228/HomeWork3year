package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.MessageModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class MessageApiClient extends BaseApiClient {

    public MessageApiClient(RestTemplate restTemplate,
                            @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/message");
    }

    public List<MessageModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<MessageModel>>() {});
    }

    public MessageModel findById(int id) {
        return getOne("/" + id, MessageModel.class);
    }

    public List<MessageModel> findBySenderId(int senderId) {
        return findAll().stream()
                .filter(m -> m.getSender() != null && m.getSender().getId() == senderId)
                .toList();
    }

    public MessageModel addMessage(MessageModel message) {
        MultiValueMap<String, String> params = form();
        add(params, "senderId", message.getSender() != null ? message.getSender().getId() : null);
        add(params, "receiverId", message.getReceiver() != null ? message.getReceiver().getId() : null);
        add(params, "text", message.getText());
        return postForm("", params, MessageModel.class);
    }

    public void updateMessage(MessageModel message) {
        MultiValueMap<String, String> params = form();
        add(params, "id", message.getId());
        add(params, "senderId", message.getSender() != null ? message.getSender().getId() : null);
        add(params, "receiverId", message.getReceiver() != null ? message.getReceiver().getId() : null);
        add(params, "text", message.getText());
        putForm("", params);
    }

    public void deleteMessage(int id) {
        delete("/" + id);
    }
}