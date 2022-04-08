
import win32evtlog
import networkx as nx
import xml.etree.ElementTree as ET
import sys
import json

class Grapher():

    def __init__(self):
        self.G = nx.Graph()

    def graph_net(self, event):
       
        flow = "{0}:{1}:{2}".format(event['DestAddr'], event['EventName'], event['DestPort'])
        dst_ip = event['DestAddr']
 
        self.add_node(flow, 'flow')
        self.add_node(dst_ip, 'ip')

        self.add_edge(flow, dst_ip, 'flow_ip')

        try:
            path = event['Filename']['Path']
            md5 = event['Filename']['Hashes']['Md5']
            self.add_node(path, 'path')
            self.add_node(md5, 'md5')
            self.add_edge(path, md5, 'path_md5')
            self.add_edge(md5, flow, 'md5_flow')

        except:
            process_name = event['ProcessName']
            self.add_node(process_name, 'process')
            self.add_edge(process_name, flow, 'process_ip')
            pass

    def graph_dns(self, event):
        query_name = event['QueryName']

        self.add_node(query_name, 'qname')
        for ip in event['IpEntities']:
            if ip.startswith('::ffff:'):
                f_ip = ip.split(':')[-1]
            else:
                f_ip = ip
            self.add_node(f_ip, 'ip')
            self.add_edge(f_ip, query_name, 'dns')
            
    def add_node(self, node, type):
        if node not in self.G:
            self.G.add_node(node, rel_type=type)

    def add_edge(self, src, dst, type):
        if self.G.has_edge(src, dst) == False:
            self.G.add_edge(src, dst, node_type=type)

    def write_gefx(self, filename='graph.gexf'):
        nx.write_gexf(self.G, filename)



if __name__ == "__main__":

    graph = Grapher()

    query_handle = win32evtlog.EvtQuery(
        "C:\\Windows\\System32\\winevt\\Logs\\NetProcMonLog.evtx",
        win32evtlog.EvtQueryFilePath)

    read_count = 0
    while True:
        events = win32evtlog.EvtNext(query_handle, 1000) # read in chunks of 1000
        read_count += len(events)
        if len(events) == 0:
            break
        for event in events:
            xml_content = win32evtlog.EvtRender(event, win32evtlog.EvtRenderEventXml)
            xml = ET.fromstring(xml_content)
            ns = '{http://schemas.microsoft.com/win/2004/08/events/event}'

            event_id = int(xml.find(f'.//{ns}EventID').text)
            event_data = xml.find(f'.//{ns}Data').text
            event_dict = json.loads(event_data)

            if event_id == 1 or event_id == 2:
                graph.graph_net(event_dict)
            if event_id == 3:
                graph.graph_dns(event_dict)
    graph.write_gefx()
      




